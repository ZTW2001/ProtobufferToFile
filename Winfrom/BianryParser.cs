using BianryParser.Protobuffer;
using BianryParser.Winfrom;
using System.Reflection;

namespace BianryParser
{
    public partial class MainFrom : Form
    {
        public MainFrom()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            DataPanel.Height = 0;
            ProtobufferManage.Instance.GetTypeEvent += AddPanel;
            ProtobufferManage.Instance.RefreshEvent += ClearPanel;
            ProtobufferManage.Instance.SaveEvent += SaveData;
        }

        //子面板列表
        private FromPanel? panel;
        //当前滚动位置
        private int scrollValue = 0;

        //清空面板
        private void ClearPanel()
        {
            scrollValue = this.VerticalScroll.Value;
            DataPanel.Controls.Clear();
            DataPanel.Height = 0;
        }

        //保存数据修改
        private void SaveData()
        {
            panel?.SaveData();
        }

        //创建一个新面板
        private void AddPanel(string panelName, object proto, List<PropertyInfo> infos)
        {
            panel = new(DataPanel, proto, panelName, infos);
            this.VerticalScroll.Value = scrollValue;
        }

        private void MainFrom_Load(object sender, EventArgs e)
        {
            //初始化添加所有proto类型
            foreach (Type item in ProtobufferManage.Instance.Types)
            {
                ProtoTypes.Items.Add(item);
            }
            SaveBtn.Enabled = false;
        }

        //获取文件路径并输出
        private void ReadBtn_Click(object sender, EventArgs e)
        {
            if (ProtoTypes.SelectedItem == null)
            {
                MessageBox.Show("请选择Proto类型", "提示");
                return;
            }
            string? filePath = ToolExpand.OpenFile();
            if (filePath != null)
            {
                FilePath.Text = filePath;
                ProtobufferManage.Instance.ReadProto((Type)ProtoTypes.SelectedItem, filePath);
                SaveBtn.Enabled = true;
            }
        }

        //保存文件
        private void SaveBtn_Click(object sender, EventArgs e)
        {
            ProtobufferManage.Instance.SaveProto((Type)ProtoTypes.SelectedItem, FilePath.Text);
        }

        //创建新文件
        private void CreateBtn_Click(object sender, EventArgs e)
        {
            if (ProtoTypes.SelectedItem == null)
            {
                MessageBox.Show("请选择Proto类型", "提示");
                return;
            }
            string? filePath = ToolExpand.OpenFolder();
            if (filePath != null)
            {
                FilePath.Text = filePath;
                ProtobufferManage.Instance.CreateProto((Type)ProtoTypes.SelectedItem);
                SaveBtn.Enabled = true;
            }
        }

        //当ComboBox值改变时修改选中类型
        private object? type;
        private void ProtoType_Changed(object sender, EventArgs e)
        {
            object type = ((ComboBox)sender).SelectedItem;
            if (type != null)
            {
                if (this.type != type)
                {
                    this.type = type;
                    SaveBtn.Enabled = false;
                }
            }
        }
    }
}