using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SOLID.PrettyFormatter;

namespace SOLID
{
    public class Program //SOLID prensiplerini anlamak için oluþturulan öernek kodlar
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            var jsonFormatter = new JsonFormatter();
            PrettyFormatter formatter = new PrettyFormatter(jsonFormatter);
            var FormattedText=formatter.Format(@"{""id"":1,""ad"":""Ali"",""soyad"":""Özgür""}");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

        public class PrettyFormatter //  PrettyFormatter: Bu class içinde formatterlarýn ayrý ayrý her biri kendi iþlemini yapacak þekilde oluþturulmasý yapýlacak deðiþiklikler olduðunda sadece istenilen kýsýmda yapýlmasý yeterli 
    {
            private PrettyFormatProvider _provider; // PrettyFormatprovider tipinde deðiþken tanýmlanmasý 
            public PrettyFormatter(PrettyFormatProvider provider) // contructor oluþturulmasý (Yapýcý Metot)
            {
                _provider = provider;

            }
            public string Format(string input) //Format iþlemini yapan metot tanýmlama  
            {
                return _provider.Format(input);
            }

        #region Providers  // Providerslarýn oluþturulmasý 
        public abstract class PrettyFormatProvider
            {
                public abstract string Format(string input);
            }
            public class JsonFormatter : PrettyFormatProvider
            {
                private IPrettyFormatValidator _validator; // IPrettyFormatValidator tipinde deðiþken tanýmlama 
            public JsonFormatter(IPrettyFormatValidator validator) // constructor oluþrurulmasý (Yapýcý metot )
                {
                    _validator = validator;
                }
                public override string Format(string input)  // Format metodunun constructor (yapýcý metot) ile uygun çalýþýr hale gelmesi 
                {
                    if (!_validator.IsValid(input)) //IPrettyFormatValidator tipindeki _validator deðiþkeniyle IsValid komutunun çalýþtýrýlmasý 
                {
                        throw new ValidationException(); // gelen sonuca göre iþlem belirlenmesi false ise ValidationException
                        return "formatlanmýþ metin"; // true ise istenilen sonuç 

                    }
                }
                public class HtmlFormatter : PrettyFormatProvider // PrettyFormatProvider dan türetilen HtmlFormatter 
            {
                    private IPrettyFormatValidator _validator; // IPrettyFormatValidator tipinde deðiþkenin tanýmlanmasý 
                public HtmlFormatter(IPrettyFormatValidator validator) // Constructor oluþturulmasý 
                    {
                        _validator = validator;
                    }
                    public override string Format(string input) // Constructor metodunun uygun hale gelmesi için override iþlemi
                    { 
                        if (!_validator.IsValid(input)) // Validation iþleminin yapýlmasý 
                        {
                            throw new ValidationException(); // false ise ValidationException
                        return "formatlanmýþ metin"; // true ise sonucun verilmesi
                        }
                    }
                }

                public class XmlFormatter : PrettyFormatProvider 
                {
                    private IPrettyFormatValidator _validaotr; //IPrettyFormatValidator  tipinde deðiþken tanýmlanmasý

                public XmlFormatter(IPrettyFormatValidator validator) // constructor oluþturulmasý yapýcý metot. 
                    {
                        _validator = validator;

                    }
                    public override string Format(string input) //  Constructor metodunun uygun hale gelmesi için override iþlemi 
                {
                        throw new ValidationException();
                        return "formatlanmýþ metin";

                    }
                }
            }

            #endregion 

            #region Validators  // Validation iþlemleri için 
            public class ValidationException : ApplicationException { }
            public interface IPrettyFormatValidator // Interface tanýmlanmasý 

            {
                bool IsValid(string input); // IsValid metodunun tanýmlanmasý . Interfacede tanýmlanan metotlarýn diðer subclasslarda kullanýlmasý için bu metotlarýn oluþturulmasý gerekmektedir.  
            } 

            public interface IPrettyFormatSchemaValidator : IPrettyFormatValidator //IPrettyFormatValidator interface inden türetilen bir IPrettyFormatSchemaValidator interface 
        {
                string Schema { get; set; }  // bu interface de tanýmlanan property ile diðer subclasslar içinde kullanýlabilmesi için tanýmlama yapýlýr.  
            }

            public class JsonValidator: IPrettyFormatValidator // IPrettyFormatValidator dan türetilen JsonValidator subclass
        {
                public bool IsValid(string input) //  IPrettyFormatValidator interfaceden alýnan IsValid metoduna sadece parametre verilmesi ve diðer iþlemlerin tanýmlanan yerde yapýlacak
            {
                    return true; // iþlem sonucu true olarak gönderilecek
                }
            }
            public class HtmlValidator : IPrettyFormatValidator // IsValid metodunun her biri altýnda ayrý ayrý yapýlmasý metotlar üzerinde yapýlacak deðiþiklikler için sadece o metot içinde yapýlmasý yeterli olacaktýr.  
            {                                                   // Bu sayede projenin içinde baðýmsýzlýk olmasý yapýlan deðiþikliklerde kolaylýk saðlayacaktýr. 
                public bool IsValid(string input)
                {
                    return true;
                }
            }

            public class XmlValidator : IPrettyFormatValidator
            {
                public bool IsValid(string input)
                {
                    return true;
                }
            }
            public class XmlSchemaValidator : IPrettyFormatSchemaValidator // IPrettyFormatSchemaValidator interfaceden türetilen bir subclass burda da yapýlan iþlem hem IPrettyFormatValidator hem de  IPrettyFormatSchemaValidator 
                                                                           // içindeki property nin kullanýlmasýna olanak saðlar. Çünkü IPrettyFormatSchemaValidator : IPrettyFormatValidator 'ýn subclassý XmlSchemaValidator de
                                                                           // IPrettyFormatSchemaValidator subclass ý bu sayede üst sýnýflardaki özellikler subclass a aktarýlabilir hale geliyor. 
        {
            public string Schema { get; set; }  //IPrettyFormatSchemaValidator dan 
            public bool IsValid (string input) //IPrettyFormatValidatordan aktarým saðlar bu sayede kendi içerisinde de kullanýlýr. 
            { 
                    return true; 
                }
            }
            #endregion
        }

    }

