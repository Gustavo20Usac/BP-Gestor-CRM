window.globalGrid = window.globalGrid || {};
window.taronja = window.taronja || {};
const api = window.taronja.api;

function inicializarCatalogoGrid(config) {
    if (config.lenDefault) {
        translateGrid(config.lenDefault);
    }

    const $tabla = $(`#${config.idTabla}`);

    window.globalGrid.table = $tabla.DataTable({
        paging: true,
        stateSave: true,
        destroy: true,
        lengthChange: true,
        searching: true,
        ordering: true,
        info: true,
        autoWidth: false,
        processing: true,            // muestra “Procesando…”
        deferRender: true,
        dom: 'Bfrtip',
        ajax: {
            url: config.urlDatos,
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            /**
             * Normaliza la forma del payload:
             * - { data: [...] }
             * - [...]
             * - { d: '...json...' } (ASMX/antiguo)
             */
            dataSrc: function (json) {
                try {
                    const raw = json && json.d ? JSON.parse(json.d) : json;
                    const fromPath = config.dataPath ? raw?.[config.dataPath] : raw;
                    if (Array.isArray(fromPath)) return fromPath;
                    if (Array.isArray(raw?.data)) return raw.data;
                    if (Array.isArray(raw)) return raw;
                    return []; // sin datos => tabla vacía, sin error
                } catch (e) {
                    console.warn('dataSrc parse warn:', e);
                    return [];
                }
            },
            error: function (xhr, textStatus, error) {
                console.error('Grid load error:', textStatus, error, xhr);
                // Muestra mensaje solo en error real de red/servidor
                MessageUtils?.TimedMessage?.('Error', 'No se pudo cargar la tabla', 'error');
                // Asegura que no quede en “procesando…”
                const api = $tabla.DataTable();
                api.clear().draw();
            },
            // Si tu API puede responder 204 No Content:
            statusCode: {
                204: function () {
                    const api = $tabla.DataTable();
                    api.clear().draw();
                }
            }
        },
        language: {
            emptyTable: config.emptyText || 'No hay datos para mostrar'
        },
        createdRow: config.createdRow || function () { },
        columns: config.columnas
    });

    return window.globalGrid.table;
}


function translateGrid(e) {

    if (e == 'es-ES') {
        $.extend(true, $.fn.dataTable.defaults, {
            "language": {
                "decimal": ",",
                "thousands": ".",
                "info": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
                "infoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
                "infoPostFix": "",
                "infoFiltered": "(filtrado de un total de _MAX_ registros)",
                "loadingRecords": "Cargando...",
                "lengthMenu": "Mostrar _MENU_ registros",
                "paginate": {
                    "first": "Primero",
                    "last": "Último",
                    "next": "Siguiente",
                    "previous": "Anterior"
                },
                "processing": "Procesando...",
                "search": "Buscar:",
                "searchPlaceholder": "Término de búsqueda",
                "zeroRecords": "No se encontraron resultados",
                "emptyTable": "Ningún dato disponible en esta tabla",
                "aria": {
                    "sortAscending": ": Activar para ordenar la columna de manera ascendente",
                    "sortDescending": ": Activar para ordenar la columna de manera descendente"
                },
                //only works for built-in buttons, not for custom buttons
                "buttons": {
                    "create": "Nuevo",
                    "edit": "Cambiar",
                    "remove": "Borrar",
                    "copy": "Copiar",
                    "csv": "fichero CSV",
                    "excel": "Exportar a Excel",
                    "pdf": "documento PDF",
                    "print": "Imprimir",
                    "colvis": "Visibilidad columnas",
                    "collection": "Colección",
                    "upload": "Seleccione fichero...."
                },
                "select": {
                    "rows": {
                        _: '%d filas seleccionadas',
                        0: 'clic fila para seleccionar',
                        1: 'una fila seleccionada'
                    }
                }
            }
        });
    }
}

function gridReloadFn() {
    if (window.globalGrid.table) {
        clearTimeout(window.globalGrid.reloadTimeout);
        window.globalGrid.reloadTimeout = setTimeout(() => {
            window.globalGrid.table.ajax.reload(null, false);
        }, 300);
    }
}

// Save catalog data
function saveCatalogData(config) {
    // Validate required fields
    for (let field of config.requiredFields || []) {
        if (!$(`#${field}`).val()) {
            MessageUtils.TimedMessage('Validación', `El campo '${field}' es requerido`, 'validacion');
            return;
        }
    }

    // Build data from fields
    const data = config.fields?.reduce((acc, field) => {
        acc[field] = $(`#${field}`).val();
        return acc;
    }, {}) || config.data;

    if (!data) {
        MessageUtils.TimedMessage('Error', 'Debe proveer data o fields', 'error');
        return;
    }

    window.taronja.api.saveData({ url: config.url, data })
        .then(({ data: response, status }) => {
            if (response.data.isSuccess) {
                MessageUtils.TimedMessage('Éxito', response.data.message || 'Acción realizada', 'exito');
                if (config.gridReloadFn) gridReloadFn();
                if (config.modalId) $(`#${config.modalId}`).modal('hide');
                if (config.clearFields && config.fields) {
                    config.fields.forEach(f => $(`#${f}`).val(''));
                }

            } else {
                MessageUtils.TimedMessage('Error', response.data.message || 'Error guardando datos', 'warning');
            }
        })
        .catch(err => {
            if (typeof config.onError === 'function') {
                config.onError(err);
            } else {
                MessageUtils.TimedMessage('Error', err.data || 'Error guardando datos', 'error');
            }
        });
}

function doCambioStatus() {
    cambioStatus({
        url: urlCambioStatus,
        data: { id: $("#idStatus").val() },
        modalId: 'CambioStatusModal',
        gridReloadFn: true,
        onSuccess: (response) => {
            MessageUtils.TimedMessage('Éxito', response.data || 'Estado cambiado correctamente', 'exito');
            console.log('Cambio de estado exitoso:', response);
        },
        onError: (err) => {
            MessageUtils.TimedMessage('Error', err.data || 'Error al cambiar el estado', 'error');
            console.error('Error en cambio de estado:', err);
        }
    });
}


// Change status
function cambioStatus(config) {

    window.taronja.api.postAction({ url: config.url, data: config.data })
        .then(({ data: response, status }) => {
          
            if (response.data.isSuccess) {
                MessageUtils.TimedMessage('Éxito', response.data.message || 'Acción realizada', 'exito');
                    if (config.gridReloadFn) gridReloadFn();
                    if (config.modalId) $(`#${config.modalId}`).modal('hide');
                
            } else {
                throw response;
            }
        })
        .catch(err => {
            if (typeof config.onError === 'function') {
                config.onError(err);
            } else {
                MessageUtils.TimedMessage('Error', err.data || 'Error en la acción', 'error');
            }
        });
}


// Edit catalog data
function editarCatalogo(config) {
    // Validate required fields
    for (let field of config.requiredFields || []) {
        if (!$(`#${field}`).val()) {
            MessageUtils.TimedMessage('Validación', `El campo '${field}' es requerido`, 'validacion');
            return;
        }
    }

    // Build data from fields
    const data = config.fields?.reduce((acc, field) => {
        acc[field] = $(`#${field}`).val();
        return acc;
    }, {}) || config.data;

    if (!data) {
        MessageUtils.TimedMessage('Error', 'Debe proveer data o fields', 'error');
        return;
    }

    window.taronja.api.editData({ url: config.url, data })
        .then(({ data: response, status }) => {
            if (response.data.isSuccess) {
                MessageUtils.TimedMessage('Éxito', response.data.message || 'Acción realizada', 'exito');
                if (config.gridReloadFn) gridReloadFn();
                if (config.modalId) $(`#${config.modalId}`).modal('hide');
            } else {
                throw response;
            }
        })
        .catch(err => {
            if (typeof config.onError === 'function') {
                config.onError(err);
            } else {
                MessageUtils.TimedMessage('Error', err.data || 'Error editando datos', 'error');
            }
        });
}


// Load list for select
function cargarLista(config) {
    window.taronja.api.ajaxGet(config.url, config.params || {})
        .then(({ data: result }) => {
            const items = Array.isArray(config.dataPath ? result?.[config.dataPath] : result)
                ? (config.dataPath ? result[config.dataPath] : result)
                : [];

            const $target = $(config.targetSelector);
            $target.empty();

            if (config.defaultOptionText) {
                $target.append(`<option value="">${config.defaultOptionText}</option>`);
            }

            // Si no hay elementos, no mostramos error, solo dejamos vacío
            if (items.length === 0) {
                if (typeof config.onComplete === 'function') {
                    config.onComplete(items);
                }
                return;
            }

            items.forEach(item => {
                // Filtrado opcional
                if (config.filterField && config.filterValue !== undefined) {
                    if (item[config.filterField] != config.filterValue) return;
                }

                const value = item[config.itemValueField];
                const text = item[config.itemTextField];
                $target.append(`<option value="${value}">${text}</option>`);
            });

            if (typeof config.onComplete === 'function') {
                config.onComplete(items);
            }
        })
        .catch(err => {
            // Error real en la petición
            console.error('Error cargando lista:', err);
            MessageUtils.TimedMessage('Error', 'No se pudieron cargar los datos', 'error');
        });
}


function formatDateTime(dateStr) {
    if (!dateStr) return "";
    const fecha = new Date(dateStr);
    return fecha.toLocaleString("es-ES", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit"
    });
}

function setValueIfExists(selector, value) {
    const $el = $(selector);
    if ($el.length) $el.val(value ?? "");
}

function setTextIfExists(selector, value) {
    const $el = $(selector);
    if ($el.length) $el.text(value ?? "—");
}

// ===========================
// Guardar con FormData (multipart)
// ===========================
function saveCatalogFormData(config) {
    // Validación
    for (let field of (config.requiredFields || [])) {
        const $el = $(`#${field}`);
        if (!$el.length) {
            MessageUtils.TimedMessage('Error', `No se encontró el campo '${field}'`, 'error');
            return;
        }
        const type = ($el.attr('type') || '').toLowerCase();
        if (type === 'file') {
            if (!$el[0].files || $el[0].files.length === 0) {
                MessageUtils.TimedMessage('Validación', `El campo '${field}' es requerido`, 'validacion');
                return;
            }
        } else {
            const val = $el.val();
            if (val === undefined || val === null || String(val).trim() === '') {
                MessageUtils.TimedMessage('Validación', `El campo '${field}' es requerido`, 'validacion');
                return;
            }
        }
    }

    // Construir FormData desde ids de campos
    const fd = new FormData();

    // Campos simples o archivos
    (config.fields || []).forEach(name => {
        const $el = $(`#${name}`);
        if (!$el.length) return;

        const tag = ($el.prop('tagName') || '').toLowerCase();
        const type = ($el.attr('type') || '').toLowerCase();

        if (type === 'file') {
            const file = $el[0]?.files?.[0];
            if (file) fd.append(name, file, file.name);
            else fd.append(name, ''); // si tu binder tolera vacío
        } else if (type === 'checkbox') {
            fd.append(name, $el.is(':checked') ? 'true' : 'false');
        } else {
            fd.append(name, $el.val() ?? '');
        }
    });

    // Campos extra (pares clave-valor) opcionales
    if (config.extraData && typeof config.extraData === 'object') {
        Object.entries(config.extraData).forEach(([k, v]) => fd.append(k, v ?? ''));
    }

    // Hook antes de enviar
    if (typeof config.beforeSend === 'function') {
        try { config.beforeSend(fd); } catch { /* noop */ }
    }

    // Envío con el wrapper genérico
    window.taronja.api.ajaxPostForm(config.url, fd, config.ajaxExtra)
        .then(({ data: response }) => {
            // Estructura estándar como la que usás en saveCatalogData/editarCatalogo
            const ok = response?.data?.isSuccess ?? response?.isSuccess ?? false;
            const msg = (response?.data?.message || response?.message) || 'Acción realizada';

            if (ok) {
                MessageUtils.TimedMessage('Éxito', msg, 'exito');

                if (config.gridReloadFn) gridReloadFn();
                if (config.modalId) $(`#${config.modalId}`).modal('hide');

                if (config.clearFields && Array.isArray(config.fields)) {
                    config.fields.forEach(f => {
                        const $f = $(`#${f}`);
                        const type = ($f.attr('type') || '').toLowerCase();
                        if (type === 'file') $f.val('');
                        else if (type === 'checkbox') $f.prop('checked', false);
                        else $f.val('');
                    });
                }

                if (typeof config.onSuccess === 'function') {
                    config.onSuccess(response);
                }
            } else {
                const errMsg = msg || 'Error guardando datos';
                if (typeof config.onError === 'function') config.onError(response);
                else MessageUtils.TimedMessage('Error', errMsg, 'warning');
            }
        })
        .catch(err => {
            if (typeof config.onError === 'function') config.onError(err);
            else MessageUtils.TimedMessage('Error', err?.data || 'Error en la acción', 'error');
        });
}

// ===========================
// Editar con FormData (multipart), por si lo necesitás
// ===========================
function editCatalogFormData(config) {
    // Reutilizamos la misma lógica pero permitiendo método PUT si querés
    config = Object.assign({ ajaxExtra: { method: 'POST' } }, config);
    return saveCatalogFormData(config);
}
