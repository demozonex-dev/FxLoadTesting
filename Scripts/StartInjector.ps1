param ([string][Parameter( Mandatory=$true)]$injector)

$injectorPath="I:\repos-1\FxLoadTesting\Injector\Fx.Injector.Console\bin\Debug\net7.0\Fx.Injector.Console.dll"
dotnet  $injectorPath $injector