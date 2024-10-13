﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using SQLitePCL;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProMgt.Data
{
    public class ProjectTask
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public int? SectionId { get; set; }
        public string? Description { get; set; } = string.Empty;
        public DateTime DateOfCreation { get; set; }
        public DateTime? DeadLine { get; set; }
        [Required]
        public string CreatedBy { get; set; } = String.Empty;
        public bool IsCompleted { get; set; }

        [ForeignKey("Priority")]
        public int? PriorityId { get; set; }
        public virtual Priority? Priority { get; set; }

        [ForeignKey("TaskStatus")]
        public int? TaskStatusId { get; set; }
        public virtual TaskStatus? TaskStatus { get; set; }

        [Required]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
    }

}
