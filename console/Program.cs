using Imagine;
using Newtonsoft.Json;

namespace console {
    internal class Program {

        private static void Main(string[] args) {
            var imagination = new Imagination("GPT API KEY");

            var subjects = imagination.Imagine<SchoolSubject>("wizards", 5).ToList();

            OutputToConsole(subjects);
            Console.ReadKey();
        }

        private static void OutputToConsole(object data) {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            Console.WriteLine(json);
        }

        protected class SchoolSubject {
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}