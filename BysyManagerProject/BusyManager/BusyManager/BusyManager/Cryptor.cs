using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace BusyManager
{
    public static class Cryptor
    {
        //const string sSecretKey = "#?;{??Y";
        public static void SaveData(TargetObjectsContainer<TargetObject> Data, string CryptedString, string DirPath, string FilePath)
        {
            //create directory if not exist
            DirectoryInfo dir = new DirectoryInfo(DirPath);
            if (!dir.Exists)
            {
                Directory.CreateDirectory(DirPath);
            }
            //generate encrypted key
            int Hash = CryptedString.GetHashCode();
            int Factor = 10;
            for (int i = 1; i < (Hash.ToString().Length - 8); i++)
                Factor = Factor * 10;
            string EncryptedKey = ((Hash / Factor) + (Hash % Factor)).ToString();
            //write key to file
            WriteKey(EncryptedKey, "crypt.key", App.DefaultCryptoKey);
            //serialise
            XmlSerializer ser = new XmlSerializer(typeof(TargetObjectsContainer<TargetObject>));
            MemoryStream writer = new MemoryStream();
            ser.Serialize(writer, Data);
            // Encrypt the file with key       
            WriteFile(writer, FilePath, EncryptedKey);
            //EncryptFile(writer, FilePath, EncryptedKey);
        }
        public static TargetObjectsContainer<TargetObject> LoadData(string FilePath)
        {
            string Buffer = ReadKey("crypt.key", App.DefaultCryptoKey);
            string DecryptKey = "";
            for (int i = 0; i < 8; i++)
                DecryptKey += Buffer[i];

            XmlSerializer ser = new XmlSerializer(typeof(TargetObjectsContainer<TargetObject>));

            // A FileStream is needed to read the XML document.
            //MemoryStream fs = DecryptFile(FilePath, DecryptKey);
            MemoryStream fs = ReadFile(FilePath, DecryptKey);
            XmlReader reader = XmlReader.Create(fs);

            // Declare an object variable of the type to be deserialized.
            TargetObjectsContainer<TargetObject> data;

            // Use the Deserialize method to restore the object's state.
            data = (TargetObjectsContainer<TargetObject>)ser.Deserialize(reader);
            fs.Close();

            return data;
        }
        private static string ReadKey(string sInputFilename, string sKey)
        {
            //DESCryptoServiceProvider CryptoPrivider = new DESCryptoServiceProvider();
            RC2CryptoServiceProvider CryptoPrivider = new RC2CryptoServiceProvider();
            CryptoPrivider.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            CryptoPrivider.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform decryptor = CryptoPrivider.CreateDecryptor();
            FileStream fileStream = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);
            CryptoStream cryptoStream = new CryptoStream((Stream)fileStream, decryptor, CryptoStreamMode.Read);
            byte[] numArray = new byte[fileStream.Length];
            cryptoStream.Read(numArray, 0, numArray.Length);
            cryptoStream.Close();
            fileStream.Close();
            string sOutput = Encoding.Default.GetString(numArray);
            return sOutput;
        }
        private static void WriteKey(string sInput, string sOutputFilename, string sKey)
        {
            FileStream fsEncrypted = new FileStream(sOutputFilename,
               FileMode.Create,
               FileAccess.Write);
            RC2CryptoServiceProvider CryptoPrivider = new RC2CryptoServiceProvider();
            CryptoPrivider.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            CryptoPrivider.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = CryptoPrivider.CreateEncryptor();
            CryptoStream cryptostream = new CryptoStream(fsEncrypted,
               desencrypt,
               CryptoStreamMode.Write);
            byte[] bytearrayinput = System.Text.Encoding.Default.GetBytes(sInput);
            //fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Close();
            //fsInput.Close();
            fsEncrypted.Close();
        }
        private static void EncryptFile(MemoryStream sInput, string sOutputFilename, string sKey)
        {
            FileStream fsEncrypted = new FileStream(sOutputFilename,
               FileMode.Create,
               FileAccess.Write);
            //DESCryptoServiceProvider CryptoPrivider = new DESCryptoServiceProvider();
            RC2CryptoServiceProvider CryptoPrivider = new RC2CryptoServiceProvider();
            CryptoPrivider.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            CryptoPrivider.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = CryptoPrivider.CreateEncryptor();
            CryptoStream cryptostream = new CryptoStream(fsEncrypted,
               desencrypt,
               CryptoStreamMode.Write);
            byte[] bytearrayinput = sInput.GetBuffer();
            //fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Close();
            //fsInput.Close();
            fsEncrypted.Close();
        }
        private static MemoryStream DecryptFile(string sInputFilename, string sKey)
        {
            //DESCryptoServiceProvider CryptoPrivider = new DESCryptoServiceProvider();
            RC2CryptoServiceProvider CryptoPrivider = new RC2CryptoServiceProvider();
            CryptoPrivider.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            CryptoPrivider.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform decryptor = CryptoPrivider.CreateDecryptor();
            FileStream fileStream = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);
            CryptoStream cryptoStream = new CryptoStream((Stream)fileStream, decryptor, CryptoStreamMode.Read);
            byte[] numArray = new byte[fileStream.Length];
            cryptoStream.Read(numArray, 0, numArray.Length);
            cryptoStream.Close();
            fileStream.Close();
            MemoryStream sOutput = new MemoryStream(numArray);
            //fileStream.CopyTo(sOutput);
            return sOutput;
        }
        private static void WriteFile(MemoryStream sInput, string sOutputFilename, string sKey)
        {
            FileStream fr = new FileStream(sOutputFilename, FileMode.Create);
            fr.Write(sInput.GetBuffer(), 0, sInput.GetBuffer().Length);
            fr.Close();
        }
        private static MemoryStream ReadFile(string sInputFilename, string sKey)
        {
            FileStream fr = new FileStream(sInputFilename, FileMode.Open);
            byte[] numArray = new byte[fr.Length];
            fr.Read(numArray, 0, numArray.Length);
            fr.Close();
            MemoryStream sOutput = new MemoryStream(numArray);
            //fileStream.CopyTo(sOutput);
            return sOutput;
        }
    }

    [Serializable]
    public class TargetObjectsContainer<T> //where T : IEnumerable<T>
    {
        private List<T> Content;
        public TargetObjectsContainer()
        { }
        public TargetObjectsContainer(List<T> contentList)
        {
            Content = new List<T>(contentList);
        }
        public void FillContent(List<T> contentList)
        {
            Content = new List<T>(contentList);
        }
        public List<T> ReturnContent()
        {
            return Content;
        }
    }
}

