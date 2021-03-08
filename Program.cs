using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Web;

namespace statiqtest
{
  public class Program
  {
    // public static async Task<int> Main(string[] args) =>
    //   await Bootstrapper
    //     .Factory
    //     .CreateWeb(args)
    //     .RunAsync();


    public static async Task<int> Main(string[] args){
      Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
      return await Bootstrapper
         .Factory
         .CreateWeb(args)
         .RunAsync();
    }
  }
}