// wwwroot/js/site.js
// Додатковий JavaScript код
document.addEventListener('DOMContentLoaded', function() {
    // Автоматичне закриття алертів через 5 секунд
    setTimeout(function() {
        var alerts = document.querySelectorAll('.alert');
        alerts.forEach(function(alert) {
            var fadeAlert = new bootstrap.Alert(alert);
            fadeAlert.close();
        });
    }, 5000);
});