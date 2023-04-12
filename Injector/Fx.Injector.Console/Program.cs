// See https://aka.ms/new-console-template for more information
using Fx.Injector;



IInjector injector = new Fx.Injector.EventGrid(Fx.Helpers.Configuration.Create());
//string data="{'nom':'Vernié','prenom':'Eric','age':58}";
Data data=  new Data
{
    Nom="Vernié",
    Prenom="Eric",
    Age=58
};

await injector.Send(data);
class Data
{
    public int Age { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
};

