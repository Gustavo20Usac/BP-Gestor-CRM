
const MessageUtils = (function () {
    // Mapa de íconos para los tipos de mensaje
    const iconMap = {
        exito: "success",
        validacion: "warning",
        error: "error",
    };

    /**
     * Función base para mostrar un mensaje con Swal
     * @param {Object} options Opciones para configurar el mensaje
     * @param {string} options.titulo - Título del mensaje
     * @param {string} options.mensaje - Contenido HTML o texto del mensaje
     * @param {string} options.tipo - Tipo de mensaje ('exito', 'validacion', 'error')
     * @param {number|null} options.timer - Tiempo en ms para auto cerrar, null para que no se cierre solo
     * @param {boolean} options.showCloseButton - Mostrar botón cerrar
     * @param {string} options.confirmButtonText - Texto del botón confirmar
     * @param {string} options.confirmButtonColor - Color del botón confirmar
     * @param {Function|null} options.onClose - Callback al cerrar el mensaje
     */
    function showMessage({
        titulo = "",
        mensaje = "",
        tipo = "validacion",
        timer = null,
        showCloseButton = false,
        confirmButtonText = "Entendido",
        confirmButtonColor = "#005293",
        onClose = null,
    } = {}) {
        Swal.fire({
            title: titulo,
            html: mensaje,
            icon: iconMap[tipo] || "warning",
            timer,
            showCloseButton,
            confirmButtonText,
            confirmButtonColor,
            didClose: () => {
                if (typeof onClose === "function") {
                    onClose();
                }
            },
        });
    }

    // Funciones públicas que usan showMessage con configuraciones específicas

    function Message(titulo, mensaje, tipo) {
        showMessage({ titulo, mensaje, tipo });
    }

    function TimedMessage(titulo, mensaje, tipo) {
        showMessage({ titulo, mensaje, tipo, timer: 5000 });
    }

    function TimedMessageRedirectLogin(titulo, mensaje, tipo) {
        showMessage({
            titulo,
            mensaje,
            tipo,
            timer: 5000,
            showCloseButton: true,
            onClose: () => {
                window.location.href = "/";
            },
        });
    }

    function ModalMessageRedirect(titulo, mensaje, tipo, errorValidations = "", redirectUrl = "/") {
        showMessage({
            titulo,
            mensaje: mensaje + errorValidations,
            tipo,
            showCloseButton: true,
            onClose: () => {
                window.location.href = redirectUrl;
            },
        });
    }

    return {
        Message,
        TimedMessage,
        TimedMessageRedirectLogin,
        ModalMessageRedirect,
        showMessage, // también exportamos la base para usos avanzados
    };
})();
