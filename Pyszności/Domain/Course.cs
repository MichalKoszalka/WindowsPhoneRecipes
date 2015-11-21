using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyszności.Domain
{
    class Course
    {
        public string Name { get; set; }
        public int Time { get; set; }
        public List<string> Ingredients { get; set; }
        public string Recipe { get; set; }
        public CourseType Type { get; set; }
    }

    enum CourseType { Snap, Soup, MainCourse, Dessert}
}
