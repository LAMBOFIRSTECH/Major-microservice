using Microsoft.AspNetCore.Mvc;
using Moq;
using TasksManagement_API.Controllers;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
using Xunit;

//namespace TasksManagement.Tests;
// public class UnitTestTasksManagementController 
// {
// 	const int matricule = 1;

// 	Mock<IReadTasksMethods> mockReadMethods = new Mock<IReadTasksMethods>();
// 	Mock<IWriteTasksMethods> mockWriteMethods1 = new Mock<IWriteTasksMethods>();
// 	Mock<IWriteTasksMethods> mockWriteMethods2 = new Mock<IWriteTasksMethods>();

// [Fact]
// public async Task GetTasksReturns_OkResult_1()
// {
// 	// Arrange
// 	var expectedTasksList = new List<Tache>();
// 	mockReadMethods.Setup(m => m.GetTaches()).ReturnsAsync(expectedTasksList);
// 	var controller = new TasksManagementController(mockReadMethods.Object, null!);

// 	// Act
// 	var result = await controller.GetSingleOrAllTasks();

// 	// Assert
// 	var okResult = Assert.IsType<OkObjectResult>(result);
// 	var actualTasksList = Assert.IsAssignableFrom<IEnumerable<Tache>>(okResult.Value);
// 	Assert.Equal(expectedTasksList, actualTasksList);
// }

// [Fact]
// public async Task GetTasksByIdReturns_NotFound_or_OkResult_2()
// {
// 	// Arrange
// 	mockReadMethods.SetupSequence(m => m.GetTaskById(matricule))
// 	.ReturnsAsync(new Tache() { Matricule = matricule, Titre = "titre", Summary = "summary", StartDateH = DateTime.MinValue, EndDateH = DateTime.MaxValue  })
// 	.ReturnsAsync((Tache)null!);

// 	var controller = new TasksManagementController(mockReadMethods.Object, null!);

// 	// Act
// 	var result1 = await controller.SelectTask(1);
// 	var result2 = await controller.SelectTask(2);

// 	//Assert
// 	var okResult = Assert.IsType<OkObjectResult>(result1);
// 	Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

// 	var notResult = Assert.IsType<NotFoundObjectResult>(result2);
// 	Assert.Equal(StatusCodes.Status404NotFound, notResult.StatusCode);
// }

// [Fact]
// public async Task CreateTaskReturns_BadRequest_or_Conflict_or_CorrectsData_3()
// {
// 	// Arrange
// 	var tache1 = new Tache(){Matricule = 1,Titre = "titre1",Summary = "summary1",StartDateH = DateTime.MinValue,EndDateH = DateTime.MaxValue
// 	};
// 	var tache2 = new Tache(){Matricule = 1,Titre = "titre2",Summary = "summary2",StartDateH = DateTime.MinValue,EndDateH = DateTime.MaxValue
// 	};
// 	var tache3 = new Tache(){Matricule = 2,Titre = "titre3",Summary = "summary3",StartDateH = DateTime.MinValue,EndDateH = DateTime.MaxValue
// 	};

// 	var tasks = new List<Tache> { tache1 };
// 	mockReadMethods.Setup(m => m.GetTaches()).ReturnsAsync(tasks);
// 	mockWriteMethods2.Setup(m => m.CreateTask(It.IsAny<Tache>())).ReturnsAsync((Tache)null!);
// 	var controller2 = new TasksManagementController(mockReadMethods.Object, mockWriteMethods2.Object);

// 	// Act
// 	var result2 = await controller2.CreateTask(tache2);
// 	var result3 = await controller2.CreateTask(tache3);

// 	//Assert

// 	var conflictResult = Assert.IsType<ConflictObjectResult>(result2);
// 	Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
// 	var okResult = Assert.IsType<OkObjectResult>(result3);
// 	Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
// }

// [Fact]
// public async Task DeleteTaskReturns_NotFound_or_CorrectDeleting_4()
// {
// 	//Arrange
// 	mockReadMethods.SetupSequence(m => m.GetTaskById(matricule))
// 	.ReturnsAsync(new Tache() { Matricule = matricule })
// 	.ReturnsAsync((Tache)null!);

// 	var controller = new TasksManagementController(mockReadMethods.Object, mockWriteMethods1.Object);

// 	//Act
// 	var result1 = await controller.DeleteTaskById(matricule);
// 	var result2 = await controller.DeleteTaskById(matricule);

// 	//Assert
// 	var notFound = Assert.IsType<NotFoundObjectResult>(result2);
// 	Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);

// 	var okResult = Assert.IsType<OkObjectResult>(result1);
// 	Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

// }

// [Fact]
// public async Task UpdateTaskReturns_NotFound_or_OkUpdating_5()
// {
// 	//Arrange
// 	var task1 = new Tache() { Matricule = 1,Titre = "titre1",Summary = "summary1",StartDateH = DateTime.MinValue,EndDateH = DateTime.MaxValue };
// 	var task2 = new Tache() { Matricule = 2,Titre = "titre2",Summary = "summary2",StartDateH = DateTime.MinValue,EndDateH = DateTime.MaxValue};

// 	mockReadMethods.SetupSequence(m => m.GetTaskById(matricule))
// 	.ReturnsAsync(new Tache() {  Matricule= matricule })
// 	.ReturnsAsync((Tache)null!);

// 	var controller = new TasksManagementController(mockReadMethods.Object, mockWriteMethods1.Object);

// 	//Act
// 	var result1 = await controller.UpdateTask(task1);
// 	var result2 = await controller.UpdateTask(task2);

// 	//Assert
// 	var okResult = Assert.IsType<OkObjectResult>(result1);
// 	Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

// 	var notFound = Assert.IsType<NotFoundObjectResult>(result2);
// 	Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);

// }
//}
