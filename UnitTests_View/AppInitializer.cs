using System;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace UnitTests_View
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp.Android.EnableLocalScreenshots().ApkFile(@"C:\Users\bpeterson\source\repos\ProjectOrganizer\ProjectOrganizer\ProjectOrganizer.Android\bin\Release\com.companyname.projectorganizer.apk").StartApp();
            }

            return ConfigureApp.iOS.StartApp();
        }
    }
}