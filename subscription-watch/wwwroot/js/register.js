document.getElementById("registerForm").addEventListener("submit", validateForm);

function validateForm(e) {
    const errors = [];
    const login = document.getElementById("inputLogin").value.trim();
    const fullName = document.getElementById("inputFullName").value.trim();
    const password = document.getElementById("inputPassword").value;
    const confirm = document.getElementById("inputAgainPassword").value;
    const remindDaysBefore = document.getElementById("inputRemindDaysBefore").value;
    const remindDays = parseInt(remindDaysBefore, 10);
    
    if (login === "") {
        errors.push("Login must be filled out");
    } else if (login.length < 3) {
        errors.push("Login must be at least 3 characters");
    }

    if (fullName === "") {
        errors.push("Full name must be filled out");
    } else if (fullName.length < 2)
        errors.push("Full name must be at least 2 characters");

    if (password === "") {
        errors.push("Password must be filled out");
    } else {
        if (password.length < 6) {
            errors.push("Password must be at least 6 characters");
        }
        if (password !== confirm) {
            errors.push("Passwords don't match");
        }
    }

    if (isNaN(remindDays) || remindDays < 1 || remindDays > 15)
        errors.push("Remind me must be between 1 and 15 days");

    const clientErrorDiv = document.getElementById("clientErrorSummary");

    const serverErrorDiv = document.getElementById("serverErrorSummary");
    if (serverErrorDiv !== null) serverErrorDiv.style.display = "none";

    if (errors.length > 0) {
        e.preventDefault();
        clientErrorDiv.style.display = "block";

        clientErrorDiv.innerHTML = "<ul><li>" + errors.join("</li><li>") + "</li></ul>";
    } else {
        clientErrorDiv.style.display = "none";
    }
};