using System;
using Microsoft.Data.Sqlite;

class Urun
{
    public string Ad { get; set; }
    public int Stok { get; set; }
    public decimal Fiyat { get; set; }

    public Urun(string ad, int stok, decimal fiyat)
    {
        Ad = ad;
        Stok = stok;
        Fiyat = fiyat;
    }
}

class Program
{
    static string connectionString = "Data Source=stok.db";

    static void Main()
    {
        VeritabaniHazirla();

        while (true)
        {
            Console.Clear();

            Console.WriteLine("===== STOK TAKİP SİSTEMİ =====");
            Console.WriteLine("1 - Ürün Ekle");
            Console.WriteLine("2 - Ürünleri Listele");
            Console.WriteLine("3 - Ürün Ara");
            Console.WriteLine("4 - Ürün Sil");
            Console.WriteLine("5 - Stok Güncelleme");
            Console.WriteLine("6 - Çıkış");
            Console.Write("Seçiminiz: ");

            string secim = Console.ReadLine() ?? "";

            if (secim == "1")
                UrunEkle();
            else if (secim == "2")
                UrunleriListele();
            else if (secim == "3")
                UrunAra();
            else if (secim == "4")
                UrunSil();
            else if (secim == "5")
                StokGuncelle();
            else if (secim == "6")
                break;
            else
            {
                Console.WriteLine("Geçersiz seçim!");
                Console.ReadKey();
            }
        }
    }

    static void VeritabaniHazirla()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string sql = @"
                CREATE TABLE IF NOT EXISTS Urunler (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Ad TEXT,
                    Stok INTEGER,
                    Fiyat REAL
                )";

            using (var command = new SqliteCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    static void UrunEkle()
    {
        Console.Clear();
        Console.WriteLine("===== ÜRÜN EKLE =====");

        Console.Write("Ürün adı: ");
        string ad = Console.ReadLine() ?? "";

        Console.Write("Stok miktarı: ");
        int stok = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Fiyat: ");
        decimal fiyat = decimal.Parse(Console.ReadLine() ?? "0");

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string sql = @"
                INSERT INTO Urunler (Ad, Stok, Fiyat)
                VALUES ($ad, $stok, $fiyat)";

            using (var command = new SqliteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("$ad", ad);
                command.Parameters.AddWithValue("$stok", stok);
                command.Parameters.AddWithValue("$fiyat", fiyat);

                command.ExecuteNonQuery();
            }
        }

        Console.WriteLine();
        Console.WriteLine("Ürün başarıyla eklendi!");
        Console.ReadKey();
    }

    static void UrunleriListele()
    {
        Console.Clear();
        Console.WriteLine("===== ÜRÜNLER =====");

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string sql = "SELECT Ad, Stok, Fiyat FROM Urunler";

            using (var command = new SqliteCommand(sql, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    bool urunVar = false;

                    while (reader.Read())
                    {
                        urunVar = true;

                        string ad = reader.GetString(0);
                        int stok = reader.GetInt32(1);
                        decimal fiyat = Convert.ToDecimal(reader.GetValue(2));

                        Console.WriteLine(
                            $"Ürün: {ad} | Stok: {stok} | Fiyat: {fiyat} TL");
                    }

                    if (!urunVar)
                    {
                        Console.WriteLine("Henüz ürün bulunmuyor.");
                    }
                }
            }
        }

        Console.ReadKey();
    }

    static void UrunAra()
    {
        Console.Clear();
        Console.WriteLine("===== ÜRÜN ARA =====");

        Console.Write("Aranacak ürün: ");
        string ad = Console.ReadLine() ?? "";

        Urun? urun = null;

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string sql = @"
                SELECT Ad, Stok, Fiyat
                FROM Urunler
                WHERE Ad = $ad";

            using (var command = new SqliteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("$ad", ad);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        urun = new Urun(
                            reader.GetString(0),
                            reader.GetInt32(1),
                            Convert.ToDecimal(reader.GetValue(2))
                        );
                    }
                }
            }
        }

        if (urun != null)
        {
            Console.WriteLine(
                $"Ürün: {urun.Ad} | Stok: {urun.Stok} | Fiyat: {urun.Fiyat} TL");
        }
        else
        {
            Console.WriteLine("Ürün bulunamadı.");
        }

        Console.ReadKey();
    }

    static void UrunSil()
    {
        Console.Clear();
        Console.WriteLine("===== ÜRÜN SİL =====");

        Console.Write("Silinecek ürün: ");
        string ad = Console.ReadLine() ?? "";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string sql = "DELETE FROM Urunler WHERE Ad = $ad";

            using (var command = new SqliteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("$ad", ad);

                int sonuc = command.ExecuteNonQuery();

                if (sonuc > 0)
                    Console.WriteLine("Ürün başarıyla silindi!");
                else
                    Console.WriteLine("Ürün bulunamadı.");
            }
        }

        Console.ReadKey();
    }

    static void StokGuncelle()
    {
        Console.Clear();
        Console.WriteLine("===== STOK GÜNCELLE =====");

        Console.Write("Ürün adı: ");
        string ad = Console.ReadLine() ?? "";

        Console.Write("Yeni stok miktarı: ");
        int yeniStok = int.Parse(Console.ReadLine() ?? "0");

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string sql = @"
                UPDATE Urunler
                SET Stok = $stok
                WHERE Ad = $ad";

            using (var command = new SqliteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("$stok", yeniStok);
                command.Parameters.AddWithValue("$ad", ad);

                int sonuc = command.ExecuteNonQuery();

                if (sonuc > 0)
                    Console.WriteLine("Stok başarıyla güncellendi!");
                else
                    Console.WriteLine("Ürün bulunamadı.");
            }
        }

        Console.ReadKey();
    }
}