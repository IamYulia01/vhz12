// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function openModal() {
    document.getElementById('myModal').style.display = 'block';
}

function closeModal() {
    document.getElementById('myModal').style.display = 'none';
}

// Закрытие при клике вне окна
window.onclick = function (event) {
    const modal = document.getElementById('myModal');
    if (event.target == modal) {
        closeModal();
    }
}
