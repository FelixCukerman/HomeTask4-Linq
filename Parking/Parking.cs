using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace HomeTask3_Experimental_.Parking
{
    class Parking
    {
        private static readonly Lazy<Parking> lazy = new Lazy<Parking>(() => new Parking());
        private List<Car> area;
        private List<Transaction> transaction;
        private double Profit { get; set; }
        public static int currentId = 1;
        private int parkingSpace;
        private uint timeout;
        private double fine;
        public List<Car> AllCar
        {
            get 
            {
                if(area == null)
                {
                    area = new List<Car>();
                } 
                return area;
            }
        }
        protected Parking()
        {
            area = new List<Car>();
            area.Add(new Car(1, 100, "Passenger"));

            timeout = Settings.Timeout;
            parkingSpace = Settings.ParkingSpace;
            fine = Settings.Fine;
        }

        public static Parking Create
        {
            get
            {
                return lazy.Value;
            }
        }

        public void AddCar()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            double money = r.Next(70, 200);

            string category = Console.ReadLine();
            area.Add(new Car(currentId, money, category));
            Task t = Controller(currentId-1);
            currentId++;
        }

        private void WriteToLog() //изменить на json
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream("transaction.log", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, transaction);
            }
        }

        private async Task Controller(int id)
        {

            while(true)
            {
                if (area[id].Cash < 0)
                    area[id].Cash = await Fine(id);
                else
                    area[id].Cash = await Payment(id);
            }
        }

        Task<double> Payment(int id)
        {
            return Task.Run(() =>
            {
                int price = Settings.priceList[(CarType)area[id].Category];

                Thread.Sleep((int)timeout); //исправить
                Profit += price;
                transaction.Add(new Transaction(id, price));
                WriteToLog();
                return area[id].Cash - price;
            });
        }

        Task<double> Fine(int id)
        {
            return Task.Run(() =>
            {
                int price = Settings.priceList[(CarType)area[id].Category];

                Thread.Sleep((int)timeout);  //исправить
                Profit += price * fine;
                transaction.Add(new Transaction(id, price));
                WriteToLog();
                return area[id].Cash - (price * fine);
            });
        }
    }
}