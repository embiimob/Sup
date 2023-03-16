using System;
using System.Windows.Forms;
using CommandLine;
using Newtonsoft.Json;
using SUP.P2FK;
using System.Runtime.InteropServices;
using CommandLine.Text;
using System.Collections.Generic;

namespace SUP
{

    class Magician
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int HIDE = 0;
        const int SHOW = 5;

        public static void DisappearConsole()
        {
            ShowWindow(GetConsoleWindow(), HIDE);
        }
    }


    internal static class Program
    {
       
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
                     
            var parserResult = Parser.Default.ParseArguments<CommandOptions>(args);
            if (parserResult is Parsed<CommandOptions> parsedOptions)
            {

                var options = parsedOptions.Value;
                // If the user provided the required arguments for a function, execute that function
                if (options.GetRootsByAddress)
                {
                    int _Qty = -1;
                    if (options.Qty != 0) { _Qty = options.Qty; }
                    var roots = Root.GetRootsByAddress(options.Address, options.Username, options.Password, options.Url, options.Skip, _Qty, options.VersionByte);
                    var json = JsonConvert.SerializeObject(roots);
                    Console.WriteLine(json);
                    
                }
                else if (options.GetRootByTransactionId)
                {
                   
                    var root = Root.GetRootByTransactionId(options.TransactionId, options.Username, options.Password, options.Url, options.VersionByte);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else if (options.GetPublicAddressByKeyword)
                {

                    var root = Root.GetPublicAddressByKeyword(options.Keyword, options.VersionByte);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else if (options.GetKeywordByPublicAddress)
                {

                    var root = Root.GetKeywordByPublicAddress(options.Address);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else if (options.GetObjectByAddress)
                {

                    var root = OBJState.GetObjectByAddress(options.Address,options.Username,options.Password,options.Url, options.VersionByte, options.Verbose);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else if (options.GetObjectByTransactionId)
                {

                    var root = OBJState.GetObjectByTransactionId(options.TransactionId, options.Username, options.Password, options.Url, options.VersionByte);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else if (options.GetObjectByURN)
                {

                    var root = OBJState.GetObjectByURN(options.URN, options.Username, options.Password, options.Url, options.VersionByte, options.Skip);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else if (options.GetObjectByFile)
                {

                    var root = OBJState.GetObjectByFile(options.FilePath, options.Username, options.Password, options.Url, options.VersionByte, options.Skip);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else if (options.GetObjectsByAddress)
                {

                    var root = OBJState.GetObjectsByAddress(options.Address, options.Username, options.Password, options.Url, options.VersionByte, options.Skip, options.Qty);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else if (options.GetObjectsOwnedByAddress)
                {

                    var root = OBJState.GetObjectsOwnedByAddress(options.Address, options.Username, options.Password, options.Url, options.VersionByte, options.Skip, options.Qty);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else if (options.GetObjectsCreatedByAddress)
                {

                    var root = OBJState.GetObjectsCreatedByAddress(options.Address, options.Username, options.Password, options.Url, options.VersionByte, options.Skip, options.Qty);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else if (options.GetObjectsByKeyword)
                {

                    List<string> keywords = new List<string>();
                    keywords.Add(options.Keyword);
                    var root = OBJState.GetObjectsByKeyword(keywords, options.Username, options.Password, options.Url, options.VersionByte, options.Skip, options.Qty);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else if (options.GetFoundObjects)
                {

                    var root = OBJState.GetFoundObjects(options.Username, options.Password, options.Url, options.VersionByte, options.Skip, options.Qty);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else if (options.GetKeywordsByAddress)
                {

                   
                    var root = OBJState.GetKeywordsByAddress(options.Address, options.Username, options.Password, options.Url, options.VersionByte);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else if (options.GetPublicMessagesByAddress)
                {


                    var root = OBJState.GetPublicMessagesByAddress(options.Address, options.Username, options.Password, options.Url, options.VersionByte, options.Skip, options.Qty);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else if (options.GetPrivateMessagesByAddress)
                {


                    var root = OBJState.GetPrivateMessagesByAddress(options.Address, options.Username, options.Password, options.Url, options.VersionByte, options.Skip, options.Qty);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else if (options.GetPublicKeysByAddress)
                {
                    var root = Root.GetPublicKeysByAddress(options.Address, options.Username, options.Password, options.Url);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else if (options.GetProfileByAddress)
                {

                    List<string> keywords = new List<string>();
                    keywords.Add(options.Keyword);
                    var root = PROState.GetProfileByAddress(options.Address, options.Username, options.Password, options.Url, options.VersionByte,options.Verbose, options.Skip);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else if (options.GetProfileByURN)
                {

                    List<string> keywords = new List<string>();
                    keywords.Add(options.Keyword);
                    var root = PROState.GetProfileByURN(options.URN, options.Username, options.Password, options.Url, options.VersionByte, options.Verbose, options.Skip);
                    var json = JsonConvert.SerializeObject(root);
                    Console.WriteLine(json);

                }
                else
                {
                    Magician.DisappearConsole();
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new ObjectBrowser(null));
                }
            }
            else
            {
                // Handle parsing error
            }

        }


    }
    class CommandOptions
    {
        [Option("getrootbytransactionid", Required = false, HelpText = "Get root by transaction ID")]
        public bool GetRootByTransactionId { get; set; }

        [Option("getrootsbyaddress", Required = false, HelpText = "Get roots by address")]
        public bool GetRootsByAddress { get; set; }

        [Option("getpublicaddressbykeyword", Required = false, HelpText = "Get public address by keyword")]
        public bool GetPublicAddressByKeyword { get; set; }

        [Option("getkeywordbypublicaddress", Required = false, HelpText = "Get keyword by public address")]
        public bool GetKeywordByPublicAddress { get; set; }

        [Option("getobjectbyaddress", Required = false, HelpText = "Get object by address")]
        public bool GetObjectByAddress { get; set; }

        [Option("getobjectbytransactionid", Required = false, HelpText = "Get object by transaction id")]
        public bool GetObjectByTransactionId { get; set; }

        [Option("getobjectbyurn", Required = false, HelpText = "Get object by urn")]
        public bool GetObjectByURN { get; set; }

        [Option("getobjectbyfile", Required = false, HelpText = "Get object by file")]
        public bool GetObjectByFile { get; set; }

        [Option("getobjectsbyaddress", Required = false, HelpText = "Get objects by address")]
        public bool GetObjectsByAddress { get; set; }

        [Option("getobjectsownedbyaddress", Required = false, HelpText = "Get objects owned by address")]
        public bool GetObjectsOwnedByAddress { get; set; }

        [Option("getobjectscreatedbyaddress", Required = false, HelpText = "Get objects created by address")]
        public bool GetObjectsCreatedByAddress { get; set; }

        [Option("getobjectsbykeyword", Required = false, HelpText = "Get objects by keyword")]
        public bool GetObjectsByKeyword { get; set; }

        [Option("getfoundobjects", Required = false, HelpText = "Get objects from levelDB cache")]
        public bool GetFoundObjects { get; set; }

        [Option("getkeywordsbyaddress", Required = false, HelpText = "Get keywords by address")]
        public bool GetKeywordsByAddress { get; set; }

        [Option("getpublicmessagesbyaddress", Required = false, HelpText = "Get public messages by address")]
        public bool GetPublicMessagesByAddress { get; set; }

        [Option("getprivatemessagesbyaddress", Required = false, HelpText = "Get private messages by address")]
        public bool GetPrivateMessagesByAddress { get; set; }

        [Option("getpublickeysbyaddress", Required = false, HelpText = "Get public keys by address")]
        public bool GetPublicKeysByAddress { get; set; }

        [Option("getprofilebyaddress", Required = false, HelpText = "Get profile by address")]
        public bool GetProfileByAddress { get; set; }

        [Option("getprofilebyurn", Required = false, HelpText = "Get profile by urn")]
        public bool GetProfileByURN { get; set; }

        [Option('u',"username", Required = false, HelpText = "The username for authentication")]
        public string Username { get; set; }

        [Option('p', "password", Required = false, HelpText = "The password for authentication")]
        public string Password { get; set; }

        [Option('r', "url", Required = false, HelpText = "The API URL")]
        public string Url { get; set; }

        [Option('s', "skip", Required = false, HelpText = "The number of roots to skip [default: 0]")]
        public int Skip { get; set; }

        [Option('q', "qty", Required = false, HelpText = "The number of items to return [default: -1 all]")]
        public int Qty { get; set; }

        [Option('b', "versionbyte", Required = false, HelpText = "The version byte")]
        public string VersionByte { get; set; }

        [Option('t', "tid", Required = false, HelpText = "The transaction ID to query")]
        public string TransactionId { get; set; }

        [Option('a', "address", Required = false, HelpText = "The address to query")]
        public string Address { get; set; }

        [Option('k', "keyword", Required = false, HelpText = "The keyword to query")]
        public string Keyword { get; set; }

        [Option("urn", Required = false, HelpText = "The urn to query")]
        public string URN { get; set; }

        [Option('f', "filepath", Required = false, HelpText = "The full path of file to query")]
        public string FilePath { get; set; }

        [Option("verbose", Required = false, HelpText = "output event information to leveldb")]
        public bool Verbose { get; set; }

        [Usage(ApplicationAlias = "SUP.EXE")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("get root by transaction id", new CommandOptions { GetRootByTransactionId = true, TransactionId = "6d14b0dc526a431f611f16f29d684f73e6b01f0a59a0b7b3d9b8d951091c2422", Username = "good-user", Password = "better-password", Url = "http://127.0.0.1:18332", VersionByte ="111" });
                yield return new Example("get roots by address", new CommandOptions { GetRootsByAddress = true, Address = "muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs", Username = "good-user", Password = "better-password", Url = "http://127.0.0.1:18332", VersionByte = "111" });
                yield return new Example("get public address by keyword", new CommandOptions { GetPublicAddressByKeyword = true, Keyword = "20 BYTE ASCII STRING", VersionByte = "111" });
                yield return new Example("get keyword by public address", new CommandOptions { GetKeywordByPublicAddress = true, Address = "mmw6JJrmsEZ1bwyVPKvfRFwpoJ62nJJCsV" });
                yield return new Example("get object by address", new CommandOptions { GetObjectByAddress = true, Address = "muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs", Username = "good-user", Password = "better-password", Url = "http://127.0.0.1:18332", VersionByte = "111", Verbose = true });
                yield return new Example("get object by transaction id", new CommandOptions { GetObjectByTransactionId = true, TransactionId = "69ae3a76a9de22ffad7bfb9249824512fc38e01d82e2010877ead179b50f0f77", Username = "good-user", Password = "better-password", Url = "http://127.0.0.1:18332", VersionByte = "111" });
                yield return new Example("get object by urn", new CommandOptions { GetObjectByURN = true, URN = "twitter.com", Username = "good-user", Password = "better-password", Url = "http://127.0.0.1:18332", VersionByte = "111" });
                yield return new Example("get object by file", new CommandOptions { GetObjectByFile = true, FilePath = @"C:\folder\test.jpg", Username = "good-user", Password = "better-password", Url = "http://127.0.0.1:18332", VersionByte = "111" });
                yield return new Example("get objects by address", new CommandOptions { GetObjectsByAddress = true, Address = "muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs", Username = "good-user", Password = "better-password", Url = "http://127.0.0.1:18332", VersionByte = "111", Skip = 0, Qty = -1 });
                yield return new Example("get objects owned by address", new CommandOptions { GetObjectsOwnedByAddress = true, Address = "muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs", Username = "good-user", Password = "better-password", Url = "http://127.0.0.1:18332", VersionByte = "111", Skip = 0, Qty = -1 });
                yield return new Example("get objects created by address", new CommandOptions { GetObjectsCreatedByAddress = true, Address = "muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs", Username = "good-user", Password = "better-password", Url = "http://127.0.0.1:18332", VersionByte = "111", Skip = 0, Qty = -1 });
                yield return new Example("get found objects", new CommandOptions { GetFoundObjects = true, Username = "good-user", Password = "better-password", Url = "http://127.0.0.1:18332", VersionByte = "111", Skip = 0, Qty = 9 });
                yield return new Example("get keywords by address", new CommandOptions { GetKeywordsByAddress = true, Address = "mwJDUTXksGKUmU3z9nKeMvnjNnWjEXj5rW", Username = "good-user", Password = "better-password", Url = "http://127.0.0.1:18332", VersionByte = "111"});
                yield return new Example("get public messages by address", new CommandOptions { GetPublicMessagesByAddress = true, Address = "muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs", Username = "good-user", Password = "better-password", Url = "http://127.0.0.1:18332", VersionByte = "111", Skip = 0, Qty = 12 });
                yield return new Example("get private messages by address", new CommandOptions { GetPrivateMessagesByAddress = true, Address = "muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs", Username = "good-user", Password = "better-password", Url = "http://127.0.0.1:18332", VersionByte = "111", Skip = 0, Qty = 12 });
                yield return new Example("get public keys by address", new CommandOptions { GetPublicKeysByAddress = true, Address = "muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs", Username = "good-user", Password = "better-password", Url = "http://127.0.0.1:18332" });
                yield return new Example("get profile by address", new CommandOptions { GetProfileByAddress = true, Address = "muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs", Username = "good-user", Password = "better-password", Url = "http://127.0.0.1:18332", VersionByte = "111" });
                yield return new Example("get profile by urn", new CommandOptions { GetProfileByURN = true, URN = "embii4u", Username = "good-user", Password = "better-password", Url = "http://127.0.0.1:18332", VersionByte = "111"});

            }
        }

    }
}
