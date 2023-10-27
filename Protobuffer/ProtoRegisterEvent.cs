using Google.Protobuf;
using System.Reflection;

namespace BianryParser.Protobuffer
{
    internal partial class ProtobufferManage
    {
        /// <summary>
        /// 初始化Protobuffer管理类方法
        /// </summary>
        public ProtobufferManage()
        {
            Init();
        }

        private void Init()
        {
            //获取程序集
            List<Assembly> assemblies = new(AppDomain.CurrentDomain.GetAssemblies());
            string path = AppDomain.CurrentDomain.BaseDirectory + "Import\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else
            {
                foreach (string filePath in Directory.GetFiles(path, "*.dll"))
                {
                    assemblies.Add(Assembly.LoadFrom(filePath));
                }
            }

            //Assembly.LoadFile()
            //获取所有Protobuffer生成类
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.ExportedTypes)
                {
                    //检查是否含有序列化和反序列化方法
                    MethodInfo? writeTo = type.GetMethod("WriteTo");
                    MethodInfo? calculateSize = type.GetMethod("CalculateSize");
                    PropertyInfo? parser = type.GetProperty("Parser");
                    MethodInfo? parseFrom = parser?.PropertyType.GetMethod("ParseFrom", new Type[] { typeof(byte[]) });

                    if (parseFrom != null && writeTo != null && calculateSize != null && parser != null)
                    {
                        newInstance.Add(type, () => CreaterInstance(type));
                        serializes.Add(type, (object obj) => ProtoSerialize(calculateSize, writeTo, obj));
                        deserializes.Add(type, (byte[] bytes) => ProtoDeserialize(parser, parseFrom, bytes));
                        Types.Add(type);
                    }
                }
            }
        }

        /// <summary>
        /// 创建新对象实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>实例</returns>
        public static object CreaterInstance(Type type)
        {
            object? obj = Activator.CreateInstance(type);
            return obj ?? throw new Exception($"无法创建类型为:{type}的对象");
        }

        //序列化proto对象
        private static byte[]? ProtoSerialize(MethodInfo size, MethodInfo write, object instance)
        {
            object lenth = size.Invoke(instance, null) ?? throw new Exception($"调用出现错误:{size}");

            byte[] package = new byte[(int)lenth];
            if (package.Length == 0)
            {
                MessageBox.Show("序列化对象为空", "提示");
                return null;
            }

            CodedOutputStream cos = new(package);
            write.Invoke(instance, new object[] { cos });
            return package;
        }

        //反序列化proto对象
        private static object? ProtoDeserialize(PropertyInfo info, MethodInfo func, byte[] bytes)
        {
            try
            {
                return func.Invoke(info.GetValue(null), new object[] { bytes });
            }
            catch
            {
                MessageBox.Show("读取文件类型错误", "提示");
                return null;
            }
        }
    }
}
