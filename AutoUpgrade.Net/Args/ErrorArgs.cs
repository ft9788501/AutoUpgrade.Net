namespace AutoUpgrade.Net.Args
{
    public class ErrorArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="error">错误内容</param>
        public ErrorArgs(string error)
        {
            Error = error;
        }
        /// <summary>
        /// 错误内容
        /// </summary>
        public string Error { get; set; }
    }
}
