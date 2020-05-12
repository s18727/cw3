using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


    public interface IStudentsDbService
    {
        Enrollment EnrollStudent(EnrollStudentRequest request);
        Enrollment PromoteStudents(int semester, string studies);
    }

