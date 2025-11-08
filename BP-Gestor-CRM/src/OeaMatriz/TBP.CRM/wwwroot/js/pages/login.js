
window.taronja.api = window.taronja.api || {};

const api = window.taronja.api.login;

function login() {
    const email = document.getElementById("email").value.trim();
    const password = document.getElementById("password").value;


    if (!email || !password) {
        MessageUtils.TimedMessage("Datos incompletos", "Por favor ingresa tu usuario y contraseña para continuar.", "validacion");
        return;
    }

    const btn = document.getElementById("kt_sign_in_submit");
    btn.setAttribute("data-kt-indicator", "on");

    api.doLogin(urlLogin, email, password).
        then(function (result) {
            btn.removeAttribute("data-kt-indicator");
            if (result.status) {
                //  Guardar JWT en localStorage (o sessionStorage)
                window.location.href = "/Dashboard";
            } else {
                MessageUtils.TimedMessage("Ha ocurrido un error", result.error || "Usuario o contraseña incorrectos", "error");
            }
        }).catch(function (error) {
            let message = "Por favor verifica tu usuario y contraseña e intenta nuevamente.";
            btn.removeAttribute("data-kt-indicator");
            if (error.responseJSON && error.responseJSON.errorMessages) {
                // Múltiples errores
                message = error.responseJSON.errorMessages.join("<br>");
            } else if (error.responseJSON && error.responseJSON.message) {
                // Mensaje general
                message = error.responseJSON.message;
            }
            MessageUtils.TimedMessage("Ha ocurrido un error", message, "error");

        });
}


