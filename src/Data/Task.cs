using System.ComponentModel.DataAnnotations;
namespace ToDo.Data
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime? DueDate { get; set; } = null;
        public string Name { get; set; }
        public bool IsImportant {  get; set; }
        public bool IsCompleted { get; set; }
    }
}
