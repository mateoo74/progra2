(function () {
  const diccionario = {
    "Select a definition": "Seleccionar definición",
    "Authorize": "Autorizar",
    "Available authorizations": "Autorizaciones disponibles",
    "Logout": "Cerrar sesión",
    "Close": "Cerrar",
    "Try it out": "Probar",
    "Cancel": "Cancelar",
    "Execute": "Ejecutar",
    "Clear": "Limpiar",
    "Parameters": "Parámetros",
    "No parameters": "Sin parámetros",
    "Request body": "Cuerpo de la solicitud",
    "Responses": "Respuestas",
    "Response body": "Cuerpo de la respuesta",
    "Response headers": "Encabezados de respuesta",
    "Server response": "Respuesta del servidor",
    "Curl": "Comando Curl",
    "Request URL": "URL de la solicitud",
    "Code": "Código",
    "Details": "Detalles",
    "Description": "Descripción",
    "Links": "Enlaces",
    "No links": "Sin enlaces",
    "Schemas": "Esquemas de datos",
    "Example Value": "Valor de ejemplo",
    "Schema": "Esquema",
    "Download": "Descargar",
    "Media type": "Tipo de medio",
    "Controls": "Controles",
    "Accept header": "Encabezado Accept",
    "required": "obligatorio",
    "* required": "* obligatorio"
  };

  function traducirTodo(raiz) {
    const walker = document.createTreeWalker(raiz, NodeFilter.SHOW_TEXT, null, false);
    let nodo;
    while ((nodo = walker.nextNode())) {
      const limpio = nodo.nodeValue.trim();
      if (limpio && diccionario[limpio]) {
        nodo.nodeValue = nodo.nodeValue.replace(limpio, diccionario[limpio]);
      }
    }
  }

  function iniciar() {
    traducirTodo(document.body);
    new MutationObserver((mutaciones) => {
      for (const m of mutaciones) {
        m.addedNodes.forEach((n) => {
          if (n.nodeType === 1) traducirTodo(n);
          else if (n.nodeType === 3) {
            const limpio = n.nodeValue.trim();
            if (limpio && diccionario[limpio]) {
              n.nodeValue = n.nodeValue.replace(limpio, diccionario[limpio]);
            }
          }
        });
      }
    }).observe(document.body, { childList: true, subtree: true });
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", iniciar);
  } else {
    iniciar();
  }
})();