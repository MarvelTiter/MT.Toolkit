namespace MT.Toolkit.LogTool.DbLogger
{
    public interface IDbLogger<TData>
    {
        void Log(LogInfo<TData> data);
    }
}
