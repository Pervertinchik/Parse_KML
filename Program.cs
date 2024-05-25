namespace Parse_KML
{
    using System.IO;
    using System;
    using System.Collections.Generic;


    public class Program
    {
        private static string _path_kml; //Файл, который будем парсить
        public static string Path_kml
        {
            get { return _path_kml; }
            set { _path_kml = value; }
        }

        private static string _path_create; //Создаваемый файл
        public static string Path_create
        {
            get { return _path_create; }
            set { _path_create = value; }
        }


        static List<string> coordinates_collection = new List<string>(); // Список координат для преобразования
        static List<string> name_collection = new List<string>();  // Здесь хранятся собранные из KML координаты



        static void Main(string[] args)
        {
            start(); //Нужен для считывания строк пути файлов 
            open_file(Path_kml);
            create_file(Path_create);

        }

        static void start()
        {
            Console.WriteLine("Парсинг KML координат в текстовый вид в форме DMS.\n Введите адрес KML. \nНапример: C:\\Users\\user\\old_file.kml");
            Path_kml = Console.ReadLine();

            Console.WriteLine("Теперь введите адрес куда сохранить файл, при этом введите название файла и расширение в конце \nНапример: C:\\Users\\user\\new_file.txt");
            Path_create = Console.ReadLine();

        }

        static void create_file(string path)
        {

            using (StreamWriter sw = new StreamWriter(new FileStream(path, FileMode.Create)))
            {
                for (int n = 0; n < name_collection.Count; n++)
                {
                    sw.WriteLine(name_collection[n]);
                    sw.WriteLine(Translate_coord_to_dms(coordinates_collection[n]));

                }
            }


        }

        static void open_file(string path)
        {




            bool is_first = true;

            foreach (string line in File.ReadLines(path)) // Цикл для считывания строк из KML файла
            {

                if (line.Contains("<name>") && !line.Contains("точки")) // Здесь берём названия точек 
                {
                    if (is_first)
                    {
                        is_first = false;
                        continue;

                    }
                    else
                    {
                        string line2 = line.Remove(0, 14);

                        line2 = line2.Remove(line2.IndexOf('<'));
                        name_collection.Add(line2);
                    }


                }

                else if (line.Contains("<coordinates>"))   // Здесь берём сами координаты
                {
                    string line2 = line.Remove(0, 23);
                    line2 = line2.Remove(line2.IndexOf("</"));
                    coordinates_collection.Add(line2);
                }
            }









        }

        public static string Translate_coord_to_dms(string coordinate)  // В этом методе координатам придаётся законченный вид 
        {



            string[] coordinates = coordinate.Split(",");

            string latitude_string = coordinates[1].Replace('.', ',');  // Широта
            string longitude_string = coordinates[0].Replace('.', ',');

            // Преобразуем полученные подстрочки в переменную с плавающей запятой
            double latitude = Convert.ToDouble(latitude_string); 
            double longitude = Convert.ToDouble(longitude_string);

            // Специальный метод, с помощью которого мы переводим координаты из формата DD'MMMM' в формат DD'MM'SS"
            String degreeDecimalMinutes = Aspose.Gis.GeoConvert.AsPointText(latitude, longitude, Aspose.Gis.PointFormats.DegreeMinutesSeconds);

            // До следующего комментария мы преобразуем получившееся значение в законченный вид
            string[] latitude_longitude = degreeDecimalMinutes.Split(',');

            string[] latitude_subs = latitude_longitude[0].Split("'");
            string[] longitude_subs = latitude_longitude[1].Split("'");

            string latitude_seconds_string = latitude_subs[1].Replace('.', ',');
            latitude_seconds_string = latitude_seconds_string.Remove(latitude_seconds_string.IndexOf('"'));

            string longitude_seconds_string = longitude_subs[1].Replace('.', ',');
            longitude_seconds_string = longitude_seconds_string.Remove(longitude_seconds_string.IndexOf('"'));



            double latitude_seconds_double = Convert.ToDouble(latitude_seconds_string);
            double longitude_seconds_double = Convert.ToDouble(longitude_seconds_string);



            //Округление секунд до 2 знаков после запятой
            latitude_seconds_double = Math.Round(latitude_seconds_double, 2, 0);   
            longitude_seconds_double = Math.Round(longitude_seconds_double, 2, 0);

            // "Подрезаем" всё лишнее
            string final_lat = String.Concat(latitude_subs[0], "'", latitude_seconds_double, "\"");
            final_lat = final_lat.TrimStart('0');
            string final_long = String.Concat(longitude_subs[0], "'", longitude_seconds_double, "\"");
            final_long = final_long.Remove(0, 2);

            string result = String.Concat(final_lat, "N ", final_long, "E");  //Добавляем N и E к широте и долготе соответственно


            return result;



        }


    }
}
