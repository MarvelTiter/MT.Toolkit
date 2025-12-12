namespace TestWeb.Components.Pages;

public partial class Counter
{
    private int currentCount = 0;

    private void IncrementCount()
    {
        currentCount++;
        //using (var _1 = Logger.BeginScope("Scope1"))
        //{
        //    using (var _2 = Logger.BeginScope("Scope2"))
        //    {
        //        Logger.LogInformation("测试");
        //    }
        //    Logger.LogInformation("测试");
        //}
        try
        {
            throw new Exception("测试");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "{Message}", ex.Message);
        }
    }
}
