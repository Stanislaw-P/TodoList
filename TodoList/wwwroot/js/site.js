$('#createTaskForm').on('submit', function (e) {
    e.preventDefault();

    var formData = {
        Title: $('#taskTitle').val(),
        Description: $('#taskDescription').val(),
        DueDate: $('#taskDueDate').val()
    };

    $.ajax({
        url: '/Todo/Create',
        type: 'POST',
        data: formData,
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (data) {
            $('#taskListContainer').html(data);

            // Закрытие модалки
            var modalEl = document.getElementById('createTaskModal');
            var modal = bootstrap.Modal.getInstance(modalEl);
            if (modal) {
                modal.hide();
            }

            $('#createTaskForm')[0].reset();
        },
        error: function (xhr) {
            alert("Ошибка: " + xhr.responseText);
        }
    });
});

$(document).on('click', '.details-task-btn', function () {
    var title = $(this).data('title');
    var description = $(this).data('description');
    var is_completed = $(this).data('is_completed');

    $('#detailsTaskTitle').text(title);
    $('#detailsTaskDescription').text(description || "Описание отсутствует");

    const statusEl = $('#detailsTaskStatus');
    if (is_completed) {
        statusEl.html('<span class="badge bg-success">Выполнено</span>');
    } else {
        statusEl.html('<span class="badge bg-warning text-dark">В процессе</span>');
    }

    var modalEl = document.getElementById('detailsTaskModal');
    var modal = new bootstrap.Modal(modalEl);
    modal.show();
});

$(document).on('click', '.edit-task-btn', function () {
    var id = $(this).data('id');
    var title = $(this).data('title');
    var description = $(this).data('description');
    var dueDate = $(this).data('dueDate');

    $('#editTaskId').val(id);
    $('#editTaskTitle').val(title);
    $('#editTaskDescription').val(description);
    $('#taskDueDate').val(dueDate);


    var modalEl = document.getElementById('editTaskModal');
    var modal = new bootstrap.Modal(modalEl);
    modal.show();
});

$('#editTaskForm').on('submit', function (e) {
    e.preventDefault();

    var formData = {
        Id: $('#editTaskId').val(),
        Title: $('#editTaskTitle').val(),
        Description: $('#editTaskDescription').val(),
        DueDate: $('#taskDueDate').val()
    };

    $.ajax({
        url: '/Todo/Edit',
        type: 'POST',
        data: formData,
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (data) {
            $('#taskListContainer').html(data);
            var modalEl = document.getElementById('editTaskModal');
            var modal = bootstrap.Modal.getInstance(modalEl);
            modal.hide();
        },
        error: function (xhr) {
            alert("Ошибка при сохранении изменений");
        }
    });
});

function loadTasks(sortOrder) {
    $('#taskListContainer').css('opacity', '0.5');
    $.ajax({
        url: '/Todo/GetTasksPartial',
        type: 'GET',
        data: { sortOrder: sortOrder },
        success: function (data) {
            $('#taskListContainer').html(data);
            $('#taskListContainer').css('opacity', '1');
        }
    });
}

function toggleTask(taskId) {
    $('#taskListContainer').css('opacity', '0.5');

    $.ajax({
        url: '/Todo/ToggleStatus',
        type: 'POST',
        data: { id: taskId },
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (data) {
            $('#taskListContainer').html(data);
            $('#taskListContainer').css('opacity', '1');
        },
        error: function (xhr) {
            $('#taskListContainer').css('opacity', '1');
            alert("Ошибка смены статуса: " + xhr.status);
        }
    });
}

function toggleTaskWithDate(taskId, selectedDate) {
    $('#taskListContainer').css('opacity', '0.5');

    $.ajax({
        url: '/Todo/ToggleStatusWithDate',
        type: 'POST',
        data: {
            id: taskId,
            selectedDate: selectedDate
        },
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (data) {
            $('#taskListContainer').html(data);
            $('#taskListContainer').css('opacity', '1');
        },
        error: function (xhr) {
            $('#taskListContainer').css('opacity', '1');
            alert("Ошибка смены статуса: " + xhr.status);
        }
    });
}

function deleteTask(taskId) {
    $('#taskListContainer').css('opacity', '0.5');

    $.ajax({
        url: '/Todo/Delete',
        type: 'DELETE',
        data: { todoItemId: taskId },
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (data) {
            $('#taskListContainer').html(data);
            $('#taskListContainer').css('opacity', '1');
        },
        error: function (xhr) {
            $('#taskListContainer').css('opacity', '1');
            alert("Ошибка смены статуса: " + xhr.status);
        }
    });
}