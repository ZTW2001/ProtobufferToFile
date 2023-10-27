using BianryParser.Protobuffer;
using System.Collections;
using System.Reflection;

namespace BianryParser.Winfrom
{
    internal class FromPanel
    {
        private class ItemInfo
        {
            public PropertyInfo? Property;

            public TextBox? TextBox;
            public ComboBox? ComboBox;
            public CheckBox? CheckBox;

            public ItemInfo(PropertyInfo? property)
            {
                Property = property;
            }
        }

        public FromPanel(Panel panel, object proto, string name, List<PropertyInfo> infos)
        {
            BasePanel = panel;
            //初始化子Panel并修改父对象参数
            this.MyPanel = new();
            MyPanel.Width = panel.Width;
            panel.Controls.Add(MyPanel);

            this.proto = proto;
            Init(name, infos, panel);
        }
        //父组件
        public readonly Panel BasePanel;
        //本体组件
        public readonly Panel MyPanel;
        //保存对象
        private readonly object proto;
        //子物体列表
        private readonly List<FromPanel> panels = new();
        //数组哈希表
        private readonly Dictionary<PropertyInfo, List<int>> arrays = new();
        //元素间隔高度
        private int Hight => 30 * itemInfos.Count + PanelsHeight();
        //控件大小
        private readonly Size size = new(150, 23);
        //元素数量
        private readonly List<ItemInfo> itemInfos = new();

        //计算子物体所占长度
        private int PanelsHeight()
        {
            int height = 0;
            foreach (FromPanel panel in panels)
            {
                height += panel.MyPanel.Height;
            }
            return height;
        }

        /// <summary>
        /// 保存已修改的数据
        /// </summary>
        public void SaveData()
        {
            foreach (var item in panels)
            {
                item.SaveData();
            }
            ProtoValueSet();
        }

        //初始化
        private void Init(string name, List<PropertyInfo> infos, Panel panel)
        {
            MyPanel.Location = new(0, panel.Height);
            AddTtile(name);
            foreach (PropertyInfo info in infos)
            {
                //暂时不能加载Map类型
                if (info.PropertyType.Name == "MapField`2") { continue; }

                if (info.PropertyType.IsClass && info.PropertyType != typeof(string))
                {
                    object? obj = info.GetValue(proto);
                    if (obj == null)
                        InstanceClass(info);
                    //如果检索到是数组成员
                    else if (obj.GetType().Name == "RepeatedField`1")
                    {
                        AddValue(info, obj.GetType().GetGenericArguments()[0], obj);
                        arrays.Add(info, new());
                        foreach (object item in (IEnumerable)obj)
                        {
                            Button btn = AddButton("删除");
                            btn.Location = new(btn.Location.X + 300, btn.Location.Y);
                            btn.Click += (object? sender, EventArgs e) => { ArrayRemove(info, item.GetType(), obj, item); };

                            if (item.GetType().IsClass)
                                panels.Add(new(MyPanel, item, item.GetType().Name, ProtobufferManage.GetAllElements(item)));
                            else
                            {
                                AddItem(item.GetType(), null, item);
                                arrays[info].Add(itemInfos.Count - 1);
                            }
                        }
                    }
                    else
                    {
                        Button btn = AddButton("删除");
                        btn.Location = new(btn.Location.X + 300, btn.Location.Y);
                        btn.Click += (object? sender, EventArgs e) => { info.SetValue(proto, null); ProtobufferManage.Instance.Refresh(); };
                        panels.Add(new(MyPanel, obj, info.Name, ProtobufferManage.GetAllElements(obj)));
                    }
                    continue;
                }
                AddItem(info);
            }

            panel.Height += MyPanel.Height;
        }

        //创建一个类实例
        private void InstanceClass(PropertyInfo info)
        {
            AddItemType(info.PropertyType);
            Button btn = AddButton("创建");
            btn.Click += (object? sender, EventArgs e) => { btn.Enabled = false; InstanceProto(info); };
            itemInfos.Add(new(info));
            MyPanel.Height = Hight;
        }

        //为Proto引用类型生成实例化
        private void InstanceProto(PropertyInfo info)
        {
            object obj = ProtobufferManage.CreaterInstance(info.PropertyType);

            info.SetValue(proto, obj);
            ProtobufferManage.Instance.Refresh();
        }

        //向数组成员添加一个元素
        private void AddValue(PropertyInfo info, Type type, object instance)
        {
            AddItemType(type);
            Button btn1 = AddButton("添加");
            btn1.Click += (object? sender, EventArgs e) => { Insert(info, type, instance); };

            itemInfos.Add(new(null));
            MyPanel.Height = Hight;
        }

        //在panel中插入一条数据
        private static void Insert(PropertyInfo info, Type type, object instance)
        {
            MethodInfo? add = info.PropertyType.GetMethod("Add", new Type[] { type });
            object obj = ProtobufferManage.CreaterInstance(type);
            add?.Invoke(instance, new object[] { obj });
            ProtobufferManage.Instance.Refresh();
        }

        //删除指定位置元素
        private static void ArrayRemove(PropertyInfo info, Type type, object instance, object value)
        {
            MethodInfo? remove = info.PropertyType.GetMethod("Remove", new Type[] { type });
            remove?.Invoke(instance, new object[] { value });
            ProtobufferManage.Instance.Refresh();
        }

        //AddItem方法重载
        private void AddItem(PropertyInfo info)
        {
            if (!info.CanWrite)
                return;
            AddItem(info.PropertyType, info, info.GetValue(proto));
        }

        //添加一个Proto参数
        private void AddItem(Type type, PropertyInfo? info, object? value)
        {
            if (!type.IsClass && type == typeof(byte))
                return;

            AddItemType(type);
            AddItemName(type.Name);
            ItemInfo item = new(info);

            if (type == typeof(bool))
                item.CheckBox = AddCheckBox((bool)(value ?? false));

            else if (type.IsEnum)
            {
                item.ComboBox = AddComboBox(Enum.GetValues(type));
                item.ComboBox.SelectedItem = value;
            }
            else
            {
                item.TextBox = AddInputDialog(value ?? "");
            }
            itemInfos.Add(item);
            MyPanel.Height = Hight;
        }

        // 修改元素中的值
        private void ProtoValueSet()
        {
            try
            {
                foreach (var item in arrays)
                {
                    object? instance = item.Key.GetValue(proto);
                    if (item.Value.Count == 0 || instance == null) { continue; }

                    item.Key.PropertyType.GetMethod("Clear")?.Invoke(instance, null);
                    Type[] types = instance.GetType().GetGenericArguments();
                    MethodInfo? Add = item.Key.PropertyType.GetMethod("Add", types);
                    foreach (var info in item.Value)
                    {
                        Add?.Invoke(instance, new object[] { GetInputValue(itemInfos[info], types[0]) ?? new() });
                    }
                }

                foreach (ItemInfo item in itemInfos)
                {
                    if (item.Property == null) { continue; }

                    object? value = GetInputValue(item, item.Property.PropertyType);
                    item.Property.SetValue(proto, value);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("修改参数类型错误");
            }
        }

        //获取自定义输入的参数
        private static object? GetInputValue(ItemInfo item, Type type)
        {
            object? value = null;
            if (item.ComboBox != null)
                value = item.ComboBox.SelectedItem;
            else if (item.CheckBox != null)
                value = item.CheckBox.Checked;
            else if (item.TextBox != null)
                value = GetTypeValue(type, item.TextBox.Text);
            return value;
        }

        //获取类型对应的参数
        private static object GetTypeValue(Type type, object value)
        {
            if (type == typeof(int)) return Convert.ToInt32(value);
            else if (type == typeof(long)) return Convert.ToInt64(value);
            else if (type == typeof(uint)) return Convert.ToUInt32(value);
            else if (type == typeof(ulong)) return Convert.ToUInt64(value);
            else if (type == typeof(float)) return Convert.ToSingle(value);
            else if (type == typeof(double)) return Convert.ToDouble(value);
            else if (type == typeof(string)) return (string)value;
            else
                throw new Exception();

        }

        //添加一个按钮
        private Button AddButton(string text)
        {
            Button button = new()
            {
                Text = text,
                Location = new(100, Hight),

            };
            MyPanel.Controls.Add(button);
            button.Show();
            return button;
        }

        //添加标题
        private void AddTtile(string name)
        {
            Label label = new()
            {
                Text = "当前类型 : " + name,
                AutoSize = true,
                ForeColor = Color.Black,
                Location = new(0, Hight),
            };
            MyPanel.Controls.Add(label);
            itemInfos.Add(new(null));
            label.Show();
        }

        //添加一个TypeLabel
        private void AddItemType(Type type)
        {
            Label label = new()
            {
                Text = type.Name,
                AutoSize = true,
                ForeColor = Color.Green,
                Location = new(0, Hight + 3),
            };
            MyPanel.Controls.Add(label);
            label.Show();
        }

        //添加一个NameLabel
        private void AddItemName(string text)
        {
            Label label = new()
            {
                Text = text + ":",
                AutoSize = true,
                ForeColor = Color.Gray,
                Location = new(100, Hight + 3),
            };
            MyPanel.Controls.Add(label);
            label.Show();
        }

        //添加一个输入框
        private TextBox AddInputDialog(object text)
        {
            TextBox input = new()
            {
                Size = size,
                Location = new(200, Hight),
                Text = text.ToString(),
            };
            MyPanel.Controls.Add(input);
            input.Show();
            return input;
        }

        //添加一个选择列表
        private ComboBox AddComboBox(Array list)
        {
            ComboBox comboBox = new()
            {
                Size = size,
                Location = new(200, Hight),
            };
            foreach (object item in list)
            {
                comboBox.Items.Add(item);
            }
            MyPanel.Controls.Add(comboBox);
            comboBox.Show();
            return comboBox;
        }

        //添加一个单选框
        private CheckBox AddCheckBox(bool b)
        {
            CheckBox checkBox = new()
            {
                Size = size,
                Location = new(200, Hight),
                Checked = b
            };
            MyPanel.Controls.Add(checkBox);
            checkBox.Show();
            return checkBox;
        }
    }
}
