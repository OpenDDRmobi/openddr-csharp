﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Le informazioni generali relative a un assembly sono controllate dal seguente 
// set di attributi. Per modificare le informazioni associate a un assembly
// occorre quindi modificare i valori di questi attributi.
[assembly: AssemblyTitle("OpenDDR Simple Console")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("OpenDDR")]
[assembly: AssemblyProduct("OpenDDR Simple API Console")]
[assembly: AssemblyCopyright("Copyright ©  2012-2017 OpenDDR LLC and others")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Se si imposta ComVisible su false, i tipi in questo assembly non saranno visibili 
// ai componenti COM. Se è necessario accedere a un tipo in questo assembly da 
// COM, impostare su true l'attributo ComVisible per tale tipo.
[assembly: ComVisible(false)]

// Se il progetto viene esposto a COM, il GUID che segue verrà utilizzato per creare l'ID della libreria dei tipi
[assembly: Guid("4842f2e8-b7dd-4546-9241-26a5f01483ef")]

// Le informazioni sulla versione di un assembly sono costituite dai seguenti quattro valori:
//
//      Numero di versione principale
//      Numero di versione secondario 
//      Numero build
//      Revisione
//
// È possibile specificare tutti i valori oppure impostare valori predefiniti per i numeri relativi alla revisione e alla build 
// utilizzando l'asterisco (*) come descritto di seguito:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.3.0")]
[assembly: AssemblyFileVersion("1.0.3.0")]

// This will cause log4net to look for a configuration file 
// called [ThisApp].exe.config in the application base 
// directory (i.e. the directory containing [ThisApp].exe) 
// The config file will be watched for changes. 
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
