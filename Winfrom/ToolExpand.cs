namespace BianryParser.Winfrom
{
    internal static class ToolExpand
    {
        static ToolExpand()
        {
            InitDialog();
        }
        //文件获取类
        private static readonly OpenFileDialog fileDialog = new();
        //文件夹获取类
        private static readonly SaveFileDialog saveDialog = new();

        private static void InitDialog()
        {
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";
            fileDialog.Multiselect = false;

            fileDialog.Title = "请选择文件夹";
            saveDialog.Filter = "所有文件(*.*)|*.*";
            saveDialog.RestoreDirectory = false;
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <returns>文件路径</returns>
        public static string? OpenFile()
        {
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                return fileDialog.FileName;
            }
            return null;
        }

        /// <summary>
        /// 获取用户自定义文件路径
        /// </summary>
        /// <returns>文件路径</returns>
        public static string? OpenFolder()
        {
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                return saveDialog.FileName;
            }
            return null;
        }
    }
}
