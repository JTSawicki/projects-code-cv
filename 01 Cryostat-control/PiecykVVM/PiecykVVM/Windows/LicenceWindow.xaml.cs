using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using PiecykM.MarkdownProcessor;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Serilog;

namespace PiecykVVM.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy LicenceWindow.xaml
    /// </summary>
    public partial class LicenceWindow : Window
    {
        public LicenceWindow()
        {
            InitializeComponent();
            Markdown markdown = new Markdown();
            browser.NavigateToString(markdown.Transform(Licence));
            browser.Navigating += Browser_Navigating;
        }

        private void Browser_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            // To kasuje event kliknięcia w link
            e.Cancel = true;
            // Otwiera link w domyślnej przeglądarce
            string url = e.Uri.AbsoluteUri.ToString();
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        private const string Licence = @"
# Piecyk Program License

**Copyright (c)**

**2022 Jan Sawicki**

https://github.com/JTSawicki

All Rights Reserved

Unauthorized use or distribution, via any medium prohibited.

Proprietaly and confidential.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Written by Jan Sawicki, October 2022

## Resources Copyright Notice (c)

2022.07.28 Icons created by Anggara - Flaticon(<https://www.flaticon.com>): info

2022.07.28 Icons created by Freepik - Flaticon(<https://www.flaticon.com>): link, error, check-mark

2022.07.28 Icons created by Gregor Cresnar - Flaticon(<https://www.flaticon.com>): settings, warning

2022.07.28 Icons created by TravisAvery - Flaticon(<https://www.flaticon.com>): cosine

2022.09.06 Icons created by Dreamstale - Flaticon(<https://www.flaticon.com>): sound-control

## Libraries Copyright Notice (c)

**.Net and WPF**

Copyright (c) .NET Foundation and Contributors

<https://github.com/dotnet>

**CommunityToolkit.Mvvm**

Copyright (c) .NET Foundation and Contributors

<https://github.com/CommunityToolkit/dotnet>

**MarkdownSharp**

Copyright (c) 2018 Stack Exchange

<https://github.com/StackExchange/MarkdownSharp>

**Serilog**

<https://github.com/serilog/serilog>

**Serilog.Exceptions**

Copyright (c) 2015 Muhammad Rehan Saeed

<https://github.com/RehanSaeed/Serilog.Exceptions>

**Oxyplot**

Copyright (c) 2014 OxyPlot contributors

<https://github.com/oxyplot/oxyplot>

**MaterialDesignInXamlToolkit**

Copyright (c) James Willock,  Mulholland Software and Contributors

<https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit>

**NModbus**

Copyright (c) 2006 Scott Alexander, 2015 Dmitry Turin

<https://github.com/NModbus/NModbus>
";
    }
}
