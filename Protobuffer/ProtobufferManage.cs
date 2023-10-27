using System.Reflection;
using Tools;

namespace BianryParser.Protobuffer
{
    internal partial class ProtobufferManage
    {
        public static readonly ProtobufferManage Instance = new();

        //读取对象
        private object? proto;
        //Proto类型列表
        public readonly List<Type> Types = new();
        //获取到新的类时
        public Action<string, object, List<PropertyInfo>>? GetTypeEvent;
        //重新获取类型时
        public Action? RefreshEvent;
        //保存数据时
        public Action? SaveEvent;
        //序列化方法列表
        private readonly Dictionary<Type, Func<object, byte[]?>> serializes = new();
        //反序列化方法列表
        private readonly Dictionary<Type, Func<byte[], object?>> deserializes = new();
        //初始化方法列表
        private readonly Dictionary<Type, Func<object>> newInstance = new();

        /// <summary>
        /// 刷新数据并调用委托
        /// </summary>
        public void Refresh()
        {
            RefreshEvent?.Invoke();
            if (proto == null)
            {
                MessageBox.Show("数据对象为空");
                return;
            }
            List<PropertyInfo> infos = GetAllElements(proto);
            GetTypeEvent?.Invoke(proto.GetType().Name, proto, infos);
        }

        /// <summary>
        /// 创建一个新数据
        /// </summary>
        public void CreateProto(Type type)
        {
            if (type == null)
            {
                MessageBox.Show("请选择Protobuf类型");
                return;
            }
            if (newInstance.TryGetValue(type, out var func))
            {
                proto = func();
                Refresh();
            }
        }

        /// <summary>
        /// 序列化并写入文件
        /// </summary>
        /// <param name="obj">序列化对象</param>
        public void SaveProto(Type type, string path)
        {
            if (proto != null && serializes.TryGetValue(type, out var func))
            {
                SaveEvent?.Invoke();
                byte[]? bytes = func(proto);
                if (bytes == null) return;
                if (FileHelper.ByteToFile(bytes, path))
                {
                    MessageBox.Show("保存成功");
                }
                else
                {
                    MessageBox.Show("保存失败");
                }
            }
        }

        /// <summary>
        /// 读取文件并输出
        /// </summary>
        /// <param name="fileName">文件名称</param>
        public void ReadProto(Type type, string fileName)
        {
            byte[]? bytes = FileHelper.FileToByte(fileName);
            if (bytes == null)
            {
                MessageBox.Show("读取文件为空");
                return;
            }
            if (deserializes.TryGetValue(type, out var func))
            {
                proto = func(bytes);
                Refresh();
            }
        }

        /// <summary>
        /// 获取proto对象中的所有元素
        /// </summary>
        /// <param name="obj">obj对象</param>
        public static List<PropertyInfo> GetAllElements(object obj)
        {
            List<PropertyInfo> infos = new();
            foreach (PropertyInfo item in obj.GetType().GetProperties())
            {
                // 如果包含 Protobuf自带属性则跳过这些属性
                if (item.Name == "Parser" || item.Name == "Descriptor")
                    continue;

                infos.Add(item);
            }
            return infos;
        }
    }
}
