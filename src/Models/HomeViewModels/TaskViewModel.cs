using System.ComponentModel.DataAnnotations;

namespace ToDo.Models.TaskViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }
        [Required]
        public string Name { get; set; }
        [Display(Name = "Is Important")]
        public bool IsImportant { get; set; }
        [Display(Name = "Is Completed")]
        public bool IsCompleted { get; set; }
    }
}
