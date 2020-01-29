namespace CheckSign
{
    /// <summary>Program exit values</summary>
    public static class ErrorValue
    {
        public const int Success = 0x0000;
        public const int CommandLineErrors = 0x0001;
        public const int FileAlreadyExists = 0x0002;
        public const int ConfigFileMissing = 0x0003;
        public const int ArgumentValidation= 0x0004;
    }

}
