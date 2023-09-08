using static System.Collections.Specialized.BitVector32;
using System.Diagnostics.Metrics;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Security.Cryptography;
using System;
using System.Runtime.InteropServices;
using System.Globalization;
using static C_BankomatApplication.Program;

namespace C_BankomatApplication
{
    public class Program
    {
        // Task 1
        //Client=> (in Bank)
        //id,name,surname,age,salary,BankCard
        //BankCard =>
        //Bankname
        //Username
        //PAN (kartin uzerindeki 16 reqemli kod) //string
        //PIN( karti bankamata daxil ederken yazdiginiz 4 reqemli kod) string
        //CVC(kartin arxasindaki 3 reqemli kod) / string 100-999(random)
        //ExpireDate (06/2023)
        //Balans //random => istenilen

        //Bank=>Clients
        //showCardBalance(pin,pan);//yoxlanish olsun

        //1.BankCard adinda bir class yaradirsiniz hansi ki, asagidaki fieldleri* var:       
        //- PAN(kartin uzerindeki 16 reqemli kod) /string
        //- PIN(siniz karti bankamata daxil ederken yazdiginiz 4 reqemli kod) /string
        //- CVC(kartin arxasindaki 3 reqemli kod) /string
        //- Expire Date(month/year (kartin etibarlilib muddeti meselen(06/22))) /string
        //- Balans /decimal

        //3. User massiv yaradirsiniz ve ora evvelceden 5 user yaradib elave edirsiniz.
        //Kart melumatlarini ozunuz  elave edirsiniz.

        //4.Proqram ise dusen kimi sizden PIN daxil etmeyinizi teleb edecek.
        //Eger daxil etdiyiniz PIN e uygun kart varsa  yazilacaq ki,
        //"{Name} {Surname} xos gelmisiniz zehmet olmasa asagidakilardan birini secerdiniz". 

        //1.Balans
        //2.Nagd pul

        //1.secilen zaman kartdaki balansi gostermelidir
        //2.seclien zaman:
        //10 AZN
        //20 AZN
        //50 AZN
        //100 AZN
        //Diger (Istediyi meblegi ozu qeyd ede biler)

        //3.Nezere alin ki, cixarmaq istediyiniz pul eger kartdaki balansdan coxdursa.
        //Exception atmalidir ki "Balansda yeterli qeder mebleg yoxdur". 
        //4.Eger yerli qeder mebleg varsa balansdan secilen qeder mebleg cixilmali ve
        //proqram yeniden menyuya qayitmalidir.Men artiq bu sefer Balans secende artiq
        //Balansdaki pulun azaldigini gormeliyem.

        //3.Edilen emeliyyatlarin siyahisi.

        //4.Hansi vaxtda hansi emeliyyat yerine yetirildiyi haqqinda melumat.
        //- son 1,5,10 gun

        //5. Kartdan karta kocurme

        //6.Bu bolmenin secdiyiniz zaman hansi karta kocurme etmek istediyinizi sorusacaq (PAN)

        //7.Eger daxil etdiyiniz PIN yoxdursa o zaman "bu PIN koda aid kart tapilmadi" yazilmalidir.
        //Ve yeniden basa qayitmali ve sizden PIN kod daxil etmeyinizi istemelidir.

        public class BankCard
        {
            public string Pan { get; set; }
            public string Pin { get; set; }
            public string CVC { get; set; }
            public string ExpireDate { get; set; }
            public decimal Balance { get; set; }
            public string BankName { get; set; }
            public string Username { get; set; }

            public string[] TransactionHistory { get; private set; } = new string[10];
            public int TransactionCount { get; private set; } = 0;

            public BankCard(string pan, string pin, string cvc, string expireDate, decimal balance, string bankName, string username)
            {
                Pan = pan;
                Pin = pin;
                CVC = cvc;
                ExpireDate = expireDate;
                Balance = balance;
                BankName = bankName;
                Username = username;
            }

            public void CheckBalance()
            {
                Console.WriteLine($"Balance: [{Balance}]-AZN");
            }

            public void Deposit(decimal amount)
            {
                if (amount <= 0)
                {
                    throw new ArgumentException("Invalid amount. Please enter a positive amount");
                }
                else
                {
                    Balance += amount;
                    AddTransaction($"[{amount}]-AZN deposited. New balance: [{Balance}]-AZN");
                }
            }

            public void Withdraw(decimal amount)
            {
                if (amount <= 0)
                {
                    throw new ArgumentException("Invalid amount. Please enter a positive amount");
                }
                else if (Balance < amount)
                {
                    throw new InvalidOperationException("Low balance. The amount you want to withdraw exceeds your balance");
                }
                else
                {
                    Balance -= amount;
                    AddTransaction($"[{amount}]-AZN withdrawn. New balance: [{Balance}]-AZN");
                }
            }

            public void TransferToCard(BankCard targetCard, decimal amount)
            {
                if (amount <= 0)
                {
                    throw new ArgumentException("Invalid amount. Please enter a positive amount");
                }
                else if (Balance < amount)
                {
                    throw new InvalidOperationException("Low balance. The amount you want to transfer exceeds your balance");
                }
                else
                {
                    Balance -= amount;
                    targetCard.Balance += amount;
                    AddTransaction($"[{amount}]-AZN transferred to {targetCard.Pan} card");
                }
            }

            private void AddTransaction(string transaction)
            {
                if (TransactionCount >= 10)
                {
                    for (int i = 0; i < TransactionCount - 1; i++)
                    {
                        TransactionHistory[i] = TransactionHistory[i + 1];
                    }
                    TransactionCount--;
                }

                TransactionHistory[TransactionCount] = transaction;
                TransactionCount++;
            }

            public void ShowTransactionHistory(int daysAgo)
            {
                DateTime currentDate = DateTime.Now;
                for (int i = TransactionCount - 1; i >= 0; i--)
                {
                    string transaction = TransactionHistory[i];
                    string[] parts = transaction.Split(']');
                    if (parts.Length >= 2)
                    {
                        string datePart = parts[0].TrimStart('[');
                        DateTime transactionDate;
                        if (DateTime.TryParseExact(datePart, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out transactionDate))
                        {
                            if ((currentDate - transactionDate).TotalDays <= daysAgo)
                            {
                                Console.WriteLine(transaction);
                            }
                        }
                    }
                }
            }

            public static BankCard SelectCardForTransfer(BankCard[] cards)
            {
                Console.WriteLine("Select the card you want to transfer to : ");

                for (int i = 0; i < cards.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {cards[i].Pan}");
                }

                int choice = -1;
                while (choice < 1 || choice > cards.Length)
                {
                    Console.Write("Enter the number of the target card : ");
                    if (int.TryParse(Console.ReadLine(), out choice))
                    {
                        if (choice < 1 || choice > cards.Length)
                        {
                            Console.WriteLine("Invalid choice. Please enter a valid card number");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a valid card number");
                    }
                }

                return cards[choice - 1];
            }
        }

        public class Client
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public int Age { get; set; }
            public double Salary { get; set; }
            public BankCard BankCard { get; set; }

            public Client(int id, string name, string surname, int age, double salary, BankCard bankCard)
            {
                Id = id;
                Name = name;
                Surname = surname;
                Age = age;
                Salary = salary;
                BankCard = bankCard;
            }
        }

        public class Bank
        {
            private Client[] clients = new Client[50];
            private int clientCount = 0;

            public void AddClient(Client client)
            {
                if (clientCount < clients.Length)
                {
                    clients[clientCount] = client;
                    clientCount++;
                }
                else
                {
                    Console.WriteLine("The bank reached the required number of customers");
                }
            }

            public void RemoveClient(Client client)
            {
                for (int i = 0; i < clientCount; i++)
                {
                    if (clients[i] == client)
                    {
                        for (int j = i; j < clientCount - 1; j++)
                        {
                            clients[j] = clients[j + 1];
                        }
                        clients[clientCount - 1] = null;
                        clientCount--;
                        return;
                    }
                }

                Console.WriteLine("No customer found");
            }

            public void ShowRecentTransactions(string pin, string pan, int daysAgo)
            {
                foreach (Client client in clients)
                {
                    if (client != null && client.BankCard != null &&
                        client.BankCard.Pin == pin && client.BankCard.Pan == pan)
                    {
                        Console.WriteLine($"Client : {client.Name} {client.Surname}");
                        client.BankCard.ShowTransactionHistory(daysAgo);
                        return;
                    }
                }

                Console.WriteLine("No matching card found for the provided PIN and PAN");
            }

            public void TransferBetweenCards(string pin, string pan)
            {
                BankCard[] targetCards = clients
                    .Where(client => client != null && client.BankCard != null &&
                        client.BankCard.Pin != pin && client.BankCard.Pan != pan)
                    .Select(client => client.BankCard)
                    .ToArray();

                if (targetCards.Length == 0)
                {
                    Console.WriteLine("No other cards found for transfer");
                    return;
                }

                BankCard targetCard = BankCard.SelectCardForTransfer(targetCards);

                Console.Write("Enter the amount to transfer : ");
                decimal transferAmount;
                if (decimal.TryParse(Console.ReadLine(), out transferAmount))
                {
                    try
                    {
                        BankCard sourceCard = null;
                        foreach (Client client in clients)
                        {
                            if (client != null && client.BankCard != null &&
                                client.BankCard.Pin == pin && client.BankCard.Pan == pan)
                            {
                                sourceCard = client.BankCard;
                                break;
                            }
                        }

                        if (sourceCard != null)
                        {
                            sourceCard.TransferToCard(targetCard, transferAmount);
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.WriteLine("The operation was performed successfully");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine("Source card not found");
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid amount entered");
                }
            }

            public void ShowCardBalance(string pin, string pan)
            {
                foreach (Client client in clients)
                {
                    if (client != null && client.BankCard != null &&
                        client.BankCard.Pin == pin && client.BankCard.Pan == pan)
                    {
                        Console.WriteLine($"Client : {client.Name} {client.Surname}");
                        client.BankCard.CheckBalance();
                        return;
                    }
                }

                Console.WriteLine("No matching card found for the provided PIN and PAN");
            }
        }


        public static void ShowBankomat()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\t\t= > = > Welcome To Kapital Bank < = < =");
            Console.Write("Please enter your PIN :");
            Console.ResetColor();
        }

        public static void ShowChoiceMenu()
        {
            Console.WriteLine("Balance          [1]");
            Console.WriteLine("Cash withdrawal  [2]");
            Console.WriteLine("Other            [3]");
            Console.WriteLine("Back             [4]");
        }
        static void Main(string[] args)
        {
            BankCard card1 = new BankCard("1234567890123451", "1111", "111", "11/31", 1000, "Kapital", "Anvar");
            BankCard card2 = new BankCard("4169584465005105", "2222", "222", "12/32", 2000, "Kapital", "Ilkin");
            BankCard card3 = new BankCard("4098345667888923", "3333", "333", "13/33", 3000, "Kapital", "Turqay");
            BankCard card4 = new BankCard("2154567898101111", "4444", "444", "14/34", 4000, "Kapital", "Ruad");
            BankCard card5 = new BankCard("3345667688332123", "5555", "555", "15/35", 5000000, "Kapital", "Ali");


            Bank bank = new Bank();
            bank.AddClient(new Client(1, "Anvar", "Mammadov", 21, 5000, card1));
            bank.AddClient(new Client(2, "Ilkin", "Medetov", 17, 6000, card2));
            bank.AddClient(new Client(3, "Turqay", "Quliyev", 21, 7000, card3));
            bank.AddClient(new Client(4, "Ruad", "Agazade", 15, 8000, card4));
            bank.AddClient(new Client(5, "Ali", "Aliyev", 23, 10000, card5));


            bool isPinCorrect = false;
            BankCard selectedCard = null;

            while (!isPinCorrect)
            {
                ShowBankomat();

                string enteredPin = Console.ReadLine();

                foreach (BankCard card in new BankCard[] { card1, card2, card3, card4, card5 })
                {
                    if (card.Pin == enteredPin)
                    {
                        isPinCorrect = true;
                        selectedCard = card;
                        Console.Clear();
                        break;
                    }
                }

                if (!isPinCorrect)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("No card found for this PIN code");
                    Console.ResetColor();
                    Thread.Sleep(600);
                    Console.Clear();
                    continue;
                }
            }

            while (true)
            {
                if(selectedCard != null)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"\t\t= >[{selectedCard.Username}]< = Welcome Select one below : ");
                    ShowChoiceMenu();
                    Console.ResetColor();
                }

                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        Console.Clear();
                        selectedCard.CheckBalance();
                        break;
                    case 2:
                        Console.Clear();
                        Console.WriteLine("1. 10 AZN");
                        Console.WriteLine("2. 20 AZN");
                        Console.WriteLine("3. 50 AZN");
                        Console.WriteLine("4. 100 AZN");
                        Console.WriteLine("5. Diger");

                        int withdrawalChoice = Convert.ToInt32(Console.ReadLine());

                        switch (withdrawalChoice)
                        {
                            case 1:
                                try
                                {
                                    if (selectedCard != null)
                                    {
                                        selectedCard.Withdraw(10);
                                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                                        Console.WriteLine("The operation was performed successfully");
                                        Console.ResetColor();
                                    }
                                }
                                catch (InvalidOperationException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            case 2:
                                try
                                {
                                    if (selectedCard != null)
                                    {
                                        selectedCard.Withdraw(20);
                                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                                        Console.WriteLine("The operation was performed successfully");
                                        Console.ResetColor();
                                    }
                                }
                                catch (InvalidOperationException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            case 3:
                                try
                                {
                                    if (selectedCard != null)
                                    {
                                        selectedCard.Withdraw(50);
                                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                                        Console.WriteLine("The operation was performed successfully");
                                        Console.ResetColor();
                                    }
                                }
                                catch (InvalidOperationException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            case 4:
                                try
                                {
                                    if (selectedCard != null)
                                    {
                                        selectedCard.Withdraw(100);
                                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                                        Console.WriteLine("The operation was performed successfully");
                                        Console.ResetColor();
                                    }
                                }
                                catch (InvalidOperationException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            case 5:
                                Console.Write("Enter the withdrawal amount :");
                                decimal customAmount = Convert.ToDecimal(Console.ReadLine());
                                try
                                {
                                    if (selectedCard != null)
                                    {
                                        selectedCard.Withdraw(customAmount);
                                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                                        Console.WriteLine("The operation was performed successfully");
                                        Console.ResetColor();
                                    }
                                }
                                catch (InvalidOperationException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            default:
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine("Invalid Selection");
                                Console.ResetColor();
                                break;
                        }
                        break;
                    case 3:

                        Console.Clear();
                        Console.WriteLine("1. Show Recent Transactions (1 day)");
                        Console.WriteLine("2. Show Recent Transactions (5 days)");
                        Console.WriteLine("3. Show Recent Transactions (10 days)");
                        Console.WriteLine("4. Transfer Between Cards");

                        int menuChoice = Convert.ToInt32(Console.ReadLine());

                        switch (menuChoice)
                        {
                            case 1:
                                if (selectedCard != null)
                                {
                                    bank.ShowRecentTransactions(selectedCard.Pin, selectedCard.Pan, 1);
                                }
                                break;
                            case 2:
                                if (selectedCard != null)
                                {
                                    bank.ShowRecentTransactions(selectedCard.Pin, selectedCard.Pan, 5);
                                }
                                break;
                            case 3:
                                if (selectedCard != null)
                                {
                                    bank.ShowRecentTransactions(selectedCard.Pin, selectedCard.Pan, 10);
                                }
                                break;
                            case 4:
                                if (selectedCard != null)
                                {
                                    bank.TransferBetweenCards(selectedCard.Pin, selectedCard.Pan);
                                }
                                break;
                            default:
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine("Invalid Selection");
                                Console.ResetColor();
                                break;
                        }
                        break;
                    case 4:
                        Console.Clear();
                        ShowBankomat();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Ivalid Selection");
                        Console.ResetColor();
                        break;
                }               
            }
        }
    }
}