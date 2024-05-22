using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

class Program
{
    static List<Contact> contacts = new List<Contact>();
    static readonly string password = "supersecurepassword";  // 請確保這個密碼足夠安全

    static void Main()
    {
        LoadContacts();

        while (true)
        {
            Console.WriteLine("選擇操作：");
            Console.WriteLine("1. 添加聯絡人");
            Console.WriteLine("2. 刪除聯絡人");
            Console.WriteLine("3. 更新聯絡人信息");
            Console.WriteLine("4. 查詢聯絡人");
            Console.WriteLine("5. 儲存聯絡人數據");
            Console.WriteLine("6. 高級搜索聯絡人");
            Console.WriteLine("0. 退出");

            int choice = Convert.ToInt32(Console.ReadLine());

            if (choice == 0)
                break;

            switch (choice)
            {
                case 1:
                    AddContact();
                    break;

                case 2:
                    DeleteContact();
                    break;

                case 3:
                    UpdateContact();
                    break;

                case 4:
                    SearchContact();
                    break;

                case 5:
                    SaveContacts();
                    break;

                case 6:
                    AdvancedSearch();
                    break;

                default:
                    Console.WriteLine("無效選擇，請重新輸入。");
                    break;
            }
        }
    }

    static void AddContact()
    {
        Console.Write("輸入姓名：");
        string name = Console.ReadLine();
        Console.Write("輸入電話：");
        string phone = Console.ReadLine();
        Console.Write("輸入電子郵件：");
        string email = Console.ReadLine();
        Console.Write("輸入城市：");
        string city = Console.ReadLine();
        Console.Write("輸入公司：");
        string company = Console.ReadLine();

        contacts.Add(new Contact { Name = name, Phone = phone, Email = email, City = city, Company = company });
        Console.WriteLine("聯絡人已添加。");
    }

    static void DeleteContact()
    {
        Console.Write("輸入要刪除的聯絡人姓名：");
        string name = Console.ReadLine();
        var contact = contacts.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (contact != null)
        {
            contacts.Remove(contact);
            Console.WriteLine("聯絡人已刪除。");
        }
        else
        {
            Console.WriteLine("未找到聯絡人。");
        }
    }

    static void UpdateContact()
    {
        Console.Write("輸入要更新的聯絡人姓名：");
        string name = Console.ReadLine();
        var contact = contacts.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (contact != null)
        {
            Console.Write("輸入新的電話（留空以保留當前值）：");
            string phone = Console.ReadLine();
            Console.Write("輸入新的電子郵件（留空以保留當前值）：");
            string email = Console.ReadLine();
            Console.Write("輸入新的城市（留空以保留當前值）：");
            string city = Console.ReadLine();
            Console.Write("輸入新的公司（留空以保留當前值）：");
            string company = Console.ReadLine();

            if (!string.IsNullOrEmpty(phone))
                contact.Phone = phone;
            if (!string.IsNullOrEmpty(email))
                contact.Email = email;
            if (!string.IsNullOrEmpty(city))
                contact.City = city;
            if (!string.IsNullOrEmpty(company))
                contact.Company = company;

            Console.WriteLine("聯絡人信息已更新。");
        }
        else
        {
            Console.WriteLine("未找到聯絡人。");
        }
    }

    static void SearchContact()
    {
        Console.Write("輸入要查詢的聯絡人姓名：");
        string name = Console.ReadLine();
        var contact = contacts.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (contact != null)
        {
            DisplayContact(contact);
        }
        else
        {
            Console.WriteLine("未找到聯絡人。");
        }
    }

    static void SaveContacts()
    {
        string json = JsonConvert.SerializeObject(contacts, Formatting.Indented);
        string encryptedData = EncryptString(json, password);
        File.WriteAllText("contacts.json", encryptedData);
        Console.WriteLine("聯絡人數據已儲存。");
    }

    static void LoadContacts()
    {
        if (File.Exists("contacts.json"))
        {
            string encryptedData = File.ReadAllText("contacts.json");
            string json = DecryptString(encryptedData, password);
            contacts = JsonConvert.DeserializeObject<List<Contact>>(json);
            Console.WriteLine("聯絡人數據已加載。");
        }
    }

    static void AdvancedSearch()
    {
        Console.WriteLine("高級搜索選項：");
        Console.WriteLine("1. 根據姓名搜索");
        Console.WriteLine("2. 根據電話搜索");
        Console.WriteLine("3. 根據電子郵件搜索");
        Console.WriteLine("4. 根據城市搜索");
        Console.WriteLine("5. 根據公司搜索");

        int choice = Convert.ToInt32(Console.ReadLine());

        Console.Write("輸入搜索關鍵字：");
        string keyword = Console.ReadLine();

        IEnumerable<Contact> results = null;

        switch (choice)
        {
            case 1:
                results = contacts.Where(c => c.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
                break;

            case 2:
                results = contacts.Where(c => c.Phone.Contains(keyword));
                break;

            case 3:
                results = contacts.Where(c => c.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase));
                break;

            case 4:
                results = contacts.Where(c => c.City.Contains(keyword, StringComparison.OrdinalIgnoreCase));
                break;

            case 5:
                results = contacts.Where(c => c.Company.Contains(keyword, StringComparison.OrdinalIgnoreCase));
                break;

            default:
                Console.WriteLine("無效選擇。");
                return;
        }

        if (results.Any())
        {
            Console.WriteLine("搜索結果：");
            foreach (var contact in results)
            {
                DisplayContact(contact);
                Console.WriteLine("-------------------");
            }
        }
        else
        {
            Console.WriteLine("未找到匹配的聯絡人。");
        }
    }

    static void DisplayContact(Contact contact)
    {
        Console.WriteLine($"姓名：{contact.Name}");
        Console.WriteLine($"電話：{contact.Phone}");
        Console.WriteLine($"電子郵件：{contact.Email}");
        Console.WriteLine($"城市：{contact.City}");
        Console.WriteLine($"公司：{contact.Company}");
    }

    static string EncryptString(string plainText, string password)
    {
        byte[] iv = new byte[16];
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(password);
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }

                    array = memoryStream.ToArray();
                }
            }
        }

        return Convert.ToBase64String(array);
    }

    static string DecryptString(string cipherText, string password)
    {
        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(cipherText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(password);
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }
}

class Contact
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string City { get; set; }
    public string Company { get; set; }
}