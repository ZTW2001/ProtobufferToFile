using System.IO;

namespace Tools
{
    /// <summary>
    /// 文件公共处理类
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// 将文件转换成byte[]数组
        /// </summary>
        /// <param name="fileUrl">文件路径文件名称</param>
        /// <returns>byte[]数组</returns>
        public static byte[]? FileToByte(string fileUrl)
        {
            try
            {
                FileStream fs = new(fileUrl, FileMode.Open, FileAccess.Read);
                byte[] byteArray = new byte[fs.Length];
                fs.Read(byteArray, 0, byteArray.Length);
                fs.Close();
                return byteArray;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将byte[]数组保存成文件
        /// </summary>
        /// <param name="byteArray">byte[]数组</param>
        /// <param name="FileUrl">保存至硬盘的文件路径</param>
        /// <returns>调用结果</returns>
        public static bool ByteToFile(byte[] byteArray, string FileUrl)
        {
            bool result;
            try
            {
                FileStream fs = new(FileUrl, FileMode.OpenOrCreate, FileAccess.Write);
                fs.Write(byteArray, 0, byteArray.Length);
                result = true;
                fs.Close();
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}
