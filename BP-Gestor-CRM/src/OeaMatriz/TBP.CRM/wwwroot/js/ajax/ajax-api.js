window.taronja = window.taronja || {};
window.taronja.api = window.taronja.api || (function () {
    // Centralized AJAX helper
    function ajaxRequest(opts) {
        const {
            url,
            method = 'GET',
            data,
            contentType = 'application/json; charset=utf-8',
            dataType = 'json',
            xhrOptions = {},
            // si no te lo pasan, por defecto: false si es FormData, true si no lo es
            processData = !(data instanceof FormData),
            cache = true,
            headers
        } = opts;

        // Evita errores cuando contentType es false (multipart)
        const isJsonContentType = (typeof contentType === 'string') && contentType.toLowerCase().includes('json');

        // No serializar FormData
        const payload = data
            ? (isJsonContentType && !(data instanceof FormData) ? JSON.stringify(data) : data)
            : undefined;

        // Header de anti-forgery excepto GET (igual que antes, pero permite override)
        const hdrs = headers ?? ((method && method.toUpperCase() === 'GET')
            ? undefined
            : { 'RequestVerificationToken': $('meta[name="request-verification-token"]').attr('content') });

        return new Promise((resolve, reject) => {
            $.ajax({
                url,
                method,
                data: payload,
                contentType,     // puede ser false para FormData
                dataType,
                processData,     // importante para FormData
                cache,           // reenvía cache (para uploads suele ir false)
                ...xhrOptions,   // deja que agregués xhr, etc.
                headers: hdrs,
                success: (response, status, xhr) => {
                    const normalized = (dataType === 'json' && response && response.d)
                        ? JSON.parse(response.d)
                        : response;
                    resolve({ data: normalized, status, xhr });
                },
                error: (jqXHR) => {
                    if (jqXHR.status === 401) {
                        const redirectWindow = window.parent || window;
                        redirectWindow.location.href = `/login.aspx?url=${encodeURIComponent(redirectWindow.location.href)}`;
                        return;
                    }
                    reject(jqXHR);
                }
            });
        });
    }


    // Generic GET request
    function ajaxGet(url, data) {
        return ajaxRequest({
            url,
            method: 'GET',
            data,
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8'
        });
    }

    function ajaxPost(url, data) {
        return ajaxRequest({ url, method: 'POST', data });
    }

    function ajaxPostForm(url, formData) {
        return ajaxRequest({
            url,
            method: 'POST',
            data: formData,
            contentType: false,   // clave para multipart
            processData: false,   // no serializar
            cache: false          // evita cachear
        });
    }


    // POST for blob/file downloads
    function ajaxPostBlobFiles(url, data) {
        return ajaxRequest({
            url,
            method: 'POST',
            data: JSON.stringify(data),
            xhrOptions: {
                xhr: () => {
                    const xhr = new XMLHttpRequest();
                    xhr.responseType = 'blob';
                    return xhr;
                }
            },
            success: (data, status, xhr) => ({
                data,
                status,
                filename: xhr.getResponseHeader('filename'),
                filetype: xhr.getResponseHeader('filetype')
            })
        });
    }

    // Generic data operation (save/edit/post)
    function performDataOperation({ url, data, fields, method = 'POST' }) {
        let dataToSend = data;
        if (!dataToSend && fields && Array.isArray(fields)) {
            dataToSend = {};
            fields.forEach(id => {
                dataToSend[id] = $(`#${id}`).val(); // Fixed string interpolation
            });
        }
        if (!dataToSend) {
            return Promise.reject(new Error('Must provide data or fields'));
        }
        return ajaxRequest({ url, method, data: dataToSend });
    }

    return {
        login: {
            doLogin: (url, CorreoElectronico, contrasena) =>
                ajaxPost(url, { CorreoElectronico, contrasena })
        },
        ajaxGet,
        ajaxPost,
        ajaxPostForm,
        ajaxPostBlobFiles,
        saveData: (config) => performDataOperation(config),
        editData: (config) => performDataOperation(config),
        postAction: (config) => performDataOperation(config)
    };
}());