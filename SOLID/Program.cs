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
    public class Program //SOLID prensiplerini anlamak i�in olu�turulan �ernek kodlar
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            var jsonFormatter = new JsonFormatter();
            PrettyFormatter formatter = new PrettyFormatter(jsonFormatter);
            var FormattedText=formatter.Format(@"{""id"":1,""ad"":""Ali"",""soyad"":""�zg�r""}");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

        public class PrettyFormatter //  PrettyFormatter: Bu class i�inde formatterlar�n ayr� ayr� her biri kendi i�lemini yapacak �ekilde olu�turulmas� yap�lacak de�i�iklikler oldu�unda sadece istenilen k�s�mda yap�lmas� yeterli 
    {
            private PrettyFormatProvider _provider; // PrettyFormatprovider tipinde de�i�ken tan�mlanmas� 
            public PrettyFormatter(PrettyFormatProvider provider) // contructor olu�turulmas� (Yap�c� Metot)
            {
                _provider = provider;

            }
            public string Format(string input) //Format i�lemini yapan metot tan�mlama  
            {
                return _provider.Format(input);
            }

        #region Providers  // Providerslar�n olu�turulmas� 
        public abstract class PrettyFormatProvider
            {
                public abstract string Format(string input);
            }
            public class JsonFormatter : PrettyFormatProvider
            {
                private IPrettyFormatValidator _validator; // IPrettyFormatValidator tipinde de�i�ken tan�mlama 
            public JsonFormatter(IPrettyFormatValidator validator) // constructor olu�rurulmas� (Yap�c� metot )
                {
                    _validator = validator;
                }
                public override string Format(string input)  // Format metodunun constructor (yap�c� metot) ile uygun �al���r hale gelmesi 
                {
                    if (!_validator.IsValid(input)) //IPrettyFormatValidator tipindeki _validator de�i�keniyle IsValid komutunun �al��t�r�lmas� 
                {
                        throw new ValidationException(); // gelen sonuca g�re i�lem belirlenmesi false ise ValidationException
                        return "formatlanm�� metin"; // true ise istenilen sonu� 

                    }
                }
                public class HtmlFormatter : PrettyFormatProvider // PrettyFormatProvider dan t�retilen HtmlFormatter 
            {
                    private IPrettyFormatValidator _validator; // IPrettyFormatValidator tipinde de�i�kenin tan�mlanmas� 
                public HtmlFormatter(IPrettyFormatValidator validator) // Constructor olu�turulmas� 
                    {
                        _validator = validator;
                    }
                    public override string Format(string input) // Constructor metodunun uygun hale gelmesi i�in override i�lemi
                    { 
                        if (!_validator.IsValid(input)) // Validation i�leminin yap�lmas� 
                        {
                            throw new ValidationException(); // false ise ValidationException
                        return "formatlanm�� metin"; // true ise sonucun verilmesi
                        }
                    }
                }

                public class XmlFormatter : PrettyFormatProvider 
                {
                    private IPrettyFormatValidator _validaotr; //IPrettyFormatValidator  tipinde de�i�ken tan�mlanmas�

                public XmlFormatter(IPrettyFormatValidator validator) // constructor olu�turulmas� yap�c� metot. 
                    {
                        _validator = validator;

                    }
                    public override string Format(string input) //  Constructor metodunun uygun hale gelmesi i�in override i�lemi 
                {
                        throw new ValidationException();
                        return "formatlanm�� metin";

                    }
                }
            }

            #endregion 

            #region Validators  // Validation i�lemleri i�in 
            public class ValidationException : ApplicationException { }
            public interface IPrettyFormatValidator // Interface tan�mlanmas� 

            {
                bool IsValid(string input); // IsValid metodunun tan�mlanmas� . Interfacede tan�mlanan metotlar�n di�er subclasslarda kullan�lmas� i�in bu metotlar�n olu�turulmas� gerekmektedir.  
            } 

            public interface IPrettyFormatSchemaValidator : IPrettyFormatValidator //IPrettyFormatValidator interface inden t�retilen bir IPrettyFormatSchemaValidator interface 
        {
                string Schema { get; set; }  // bu interface de tan�mlanan property ile di�er subclasslar i�inde kullan�labilmesi i�in tan�mlama yap�l�r.  
            }

            public class JsonValidator: IPrettyFormatValidator // IPrettyFormatValidator dan t�retilen JsonValidator subclass
        {
                public bool IsValid(string input) //  IPrettyFormatValidator interfaceden al�nan IsValid metoduna sadece parametre verilmesi ve di�er i�lemlerin tan�mlanan yerde yap�lacak
            {
                    return true; // i�lem sonucu true olarak g�nderilecek
                }
            }
            public class HtmlValidator : IPrettyFormatValidator // IsValid metodunun her biri alt�nda ayr� ayr� yap�lmas� metotlar �zerinde yap�lacak de�i�iklikler i�in sadece o metot i�inde yap�lmas� yeterli olacakt�r.  
            {                                                   // Bu sayede projenin i�inde ba��ms�zl�k olmas� yap�lan de�i�ikliklerde kolayl�k sa�layacakt�r. 
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
            public class XmlSchemaValidator : IPrettyFormatSchemaValidator // IPrettyFormatSchemaValidator interfaceden t�retilen bir subclass burda da yap�lan i�lem hem IPrettyFormatValidator hem de  IPrettyFormatSchemaValidator 
                                                                           // i�indeki property nin kullan�lmas�na olanak sa�lar. ��nk� IPrettyFormatSchemaValidator : IPrettyFormatValidator '�n subclass� XmlSchemaValidator de
                                                                           // IPrettyFormatSchemaValidator subclass � bu sayede �st s�n�flardaki �zellikler subclass a aktar�labilir hale geliyor. 
        {
            public string Schema { get; set; }  //IPrettyFormatSchemaValidator dan 
            public bool IsValid (string input) //IPrettyFormatValidatordan aktar�m sa�lar bu sayede kendi i�erisinde de kullan�l�r. 
            { 
                    return true; 
                }
            }
            #endregion
        }

    }

