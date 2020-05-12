using System.Collections.Generic;


    public class MockDbService : IDbService
    {
        private static List<Student> _students = new List<Student>
        {
            new Student{IdStudent=1, FirstName="Andrzej", LastName="Nowak", IndexNumber="s1111"},
            new Student{IdStudent=2, FirstName="Pawel", LastName="Nowak", IndexNumber="s2222"},
            new Student{IdStudent=3, FirstName="Piotr", LastName="Kowalski", IndexNumber="s1112"},
            new Student{IdStudent=4, FirstName="Monika", LastName="Lew", IndexNumber="s1113"},
            new Student{IdStudent=5, FirstName="Natalia", LastName="Kot", IndexNumber="s1114"}
        };

        public void AddStudent(Student student)
        {
            _students.Add(student);
        }

        public void DeleteStudent(int id)
        {
            _students.Remove(GetStudent(id));
        }

        public Student GetStudent(int id)
        {
            return _students.Find(x => x.IdStudent == id);
        }

        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }
    }

