namespace Admin_Student_Teacher.Models
{
    public class TeacherVM
    {
        public string TeacherId { get; set; }   // optional (for future use)

        public string Name { get; set; }         // Aparna Sharma
        public string Email { get; set; }        // Aparna@CodeQuotient.com
        public string Subject { get; set; }      // Hindi
        public string Status { get; set; }       // Assigned / Not Assigned
    }
}
