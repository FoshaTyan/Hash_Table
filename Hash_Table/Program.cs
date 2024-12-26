using System.Text;

class Item
{
    public string Code { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }

    public Item(string code, string name, int count)
    {
        Code = code;
        Name = name;
        Count = count;
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        string filePath = "HashData.txt";


        var hashTable = ReadDataFromFile(filePath);

        // Показ таблиць
        Console.WriteLine("а) Невпорядкована таблиця:");
        UnorderedTable(filePath);
        
        Console.WriteLine("\nб) Упорядкована таблиця:");
        OrderedTable(hashTable);      

        Console.WriteLine("\nв) Таблиця з розрахунковим входом:");
        CalculatedTable(hashTable);
    }

    static List<Item>[] ReadDataFromFile(string filePath)
    {
        const int tableSize = 10; 
        var hashTable = new List<Item>[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            hashTable[i] = new List<Item>();
        }

        try
        {
            foreach (var line in File.ReadLines(filePath))
            {
                var parts = line.Split(';');
                if (parts.Length == 3)
                {
                    string code = parts[0];
                    string name = parts[1];
                    if (int.TryParse(parts[2], out int count))
                    {
                        // Створюєм новий товар
                        var item = new Item(code, name, count);

                        // Знаходимо хеш-ключ
                        int hashKey = GetHashKey(code, tableSize);

                        // Якщо комірка занята шукаємо іншу
                        while (hashTable[hashKey].Count > 0 && hashTable[hashKey].Any(i => i.Code == code) == false)
                        {
                            hashKey = (hashKey + 1) % tableSize;
                        }

                        hashTable[hashKey].Add(item);
                    }
                    else
                    {
                        Console.WriteLine($"Помилка: Невірна кількість у рядку \"{line}\".");
                    }
                }
                else
                {
                    Console.WriteLine($"Помилка: Невірний формат рядка \"{line}\".");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при зчитуванні файлу: {ex.Message}");
        }

        return hashTable;
    }

    static int GetHashKey(string code, int tableSize)
    {
        if (int.TryParse(code, out int numericCode))
        {
            return numericCode % tableSize; // Остача від ділення
        }
        else
        {
            return 0;
        }
    }

    static void OrderedTable(List<Item>[] hashTable)
    {
        for (int i = 0; i < hashTable.Length; i++)
        {
            foreach (var item in hashTable[i])
            {
                Console.WriteLine($"[{i}] Код: {item.Code}, Найменування: {item.Name}, Кількість: {item.Count}");
            }
        }
    }

    static void UnorderedTable(string filePath)
    {
        try
        {
            foreach (var line in File.ReadLines(filePath))
            {
                var parts = line.Split(';');
                if (parts.Length == 3)
                {
                    string code = parts[0];
                    string name = parts[1];
                    if (int.TryParse(parts[2], out int count))
                    {
                        Console.WriteLine($"Код: {code}, Найменування: {name}, Кількість: {count}");
                    }
                    else
                    {
                        Console.WriteLine($"Помилка: Невірна кількість у рядку \"{line}\".");
                    }
                }
                else
                {
                    Console.WriteLine($"Помилка: Невірний формат рядка \"{line}\".");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при зчитуванні файлу: {ex.Message}");
        }
    }

    static void CalculatedTable(List<Item>[] hashTable)
    {
        var allItems = hashTable
            .SelectMany(x => x)
            .GroupBy(item => item.Code)
            .Select(group => new
            {
                Code = group.Key,
                Name = group.First().Name,
                TotalCount = group.Sum(item => item.Count)
            })
            .ToList();

        foreach (var entry in allItems)
        {
            Console.WriteLine($"Код: {entry.Code}, Найменування: {entry.Name}, Кількість: {entry.TotalCount}");
        }
    }
}