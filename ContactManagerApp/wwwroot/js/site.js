document.addEventListener("DOMContentLoaded", function () {
    const fileInput = document.getElementById("fileInput");
    const clearBtn = document.getElementById("clearFileBtn");
    const uploadForm = document.getElementById("uploadForm");
    const searchSelect = document.getElementById("column-select");
    const searchInput = document.getElementById("search-input");
    const table = document.getElementById("contactsTable");

    if (fileInput && clearBtn) {
        clearBtn.addEventListener("click", () => fileInput.value = "");
    }

    if (!table) return;

    table.addEventListener("click", async (e) => {
        const target = e.target;
        const row = target.closest("tr");
        if (!row) return;

        const id = row.dataset.id;

        if (target.matches(".js-delete")) {
            toggleActionButtons(row, true);
        }
        else if (target.matches(".js-cancel")) {
            toggleActionButtons(row, false);
        }
        else if (target.matches(".js-confirm")) {
            await handleConfirmDelete(row, id);
        }
        else if (target.matches(".js-save")) {
            await handleSave(row, id);
        }
    });

    table.addEventListener("input", (e) => enableSaveBtn(e.target));
    table.addEventListener("change", (e) => enableSaveBtn(e.target));

    if (uploadForm) {
        uploadForm.addEventListener("submit", async function (e) {
            e.preventDefault();
            if (!fileInput.files.length) return showToast("Please, choose file", "error");

            try {
                const response = await fetch('/Home/Upload', {
                    method: 'POST',
                    body: new FormData(this)
                });

                const result = await response.json();

                if (response.ok) {
                    showToast("File is uploaded successfully", "success");
                    setTimeout(() => window.location.href = window.location.pathname, 1500);
                } else {
                    let msg = result.message || "Download error";
                    if (Array.isArray(result.errors) && result.errors.length) {
                        msg += ":\n" + result.errors.join("\n");
                    }
                    showToast(msg, "error");
                }
            } catch (error) {
                console.error(error);
                showToast("Network error", "error");
            }
        });
    }
    function filterRows() {
        const selectedCol = searchSelect.value;
        const q = searchInput.value.toLowerCase();
        const rows = table.querySelectorAll("tbody tr");

        rows.forEach(row => {
            let match = false;

            if (selectedCol === "all") {
                let rowText = row.textContent.toLowerCase();
                const checkbox = row.querySelector("input[type='checkbox']");

                if (checkbox) {
                    rowText += " " + checkbox.checked.toString();
                }

                match = rowText.includes(q);
            }
            else {
                const cell = row.cells[selectedCol];
                const checkbox = cell.querySelector("input[type='checkbox']");

                if (checkbox) {
                    match = checkbox.checked.toString().includes(q);
                } else {
                    match = cell.textContent.toLowerCase().includes(q);
                }
            }

            row.style.display = match ? "" : "none";
        });
    }

    searchSelect.addEventListener("change", filterRows);
    searchInput.addEventListener("input", filterRows);

    let ascending = true;
    let lastCol = -1;

    table.querySelectorAll("thead th").forEach((th, index) => {
        th.addEventListener("click", () => {
            ascending = (lastCol === index) ? !ascending : true;
            lastCol = index;

            table.querySelectorAll("thead th").forEach(h => h.dataset.sort = "");
            th.dataset.sort = ascending ? "asc" : "desc";

            const tbody = table.querySelector("tbody");
            const rows = Array.from(tbody.querySelectorAll("tr"));
            const type = th.dataset.type || (index === 4 ? 'number' : index === 1 ? 'date' : 'string');

            rows.sort((a, b) => {
                const cellA = a.cells[index];
                const cellB = b.cells[index];

                let valA, valB;

                if (type === 'bool') {
                    valA = cellA.querySelector("input[type='checkbox']").checked;
                    valB = cellB.querySelector("input[type='checkbox']").checked;

                    return ascending ? (valA === valB ? 0 : valA ? 1 : -1) : (valA === valB ? 0 : valA ? -1 : 1);
                }

                valA = cellA.textContent.trim();
                valB = cellB.textContent.trim();

                if (type === 'number') {
                    return ascending ? parseFloat(valA) - parseFloat(valB) : parseFloat(valB) - parseFloat(valA);
                } else if (type === 'date') {
                    return ascending ? Date.parse(valA) - Date.parse(valB) : Date.parse(valB) - Date.parse(valA);
                } else {
                    return ascending ? valA.localeCompare(valB) : valB.localeCompare(valA);
                }
            });

            tbody.append(...rows);
        });
    });

    table.addEventListener("input", (e) => {
        if (e.target.isContentEditable) {
            const row = e.target.closest("tr");
            const btn = document.getElementById(`save-${row.dataset.id}`);
            if (btn) btn.disabled = false;
        }
    });
});

function toggleActionButtons(row, isDeleting) {
    row.querySelector(".js-delete").hidden = isDeleting;
    row.querySelector(".js-save").hidden = isDeleting;
    row.querySelector(".js-confirm").hidden = !isDeleting;
    row.querySelector(".js-cancel").hidden = !isDeleting;
}

function enableSaveBtn(target) {
    const row = target.closest("tr");
    if (!row) return;
    const saveBtn = row.querySelector(".js-save");
    if (saveBtn) saveBtn.disabled = false;
}

async function handleConfirmDelete(row, id) {
    if (await handleRequest(`/Home/Delete?contactId=${id}`, { method: "DELETE" })) {
        row.classList.add("fade-out");
        setTimeout(() => row.remove(), 400);
    } else {
        toggleActionButtons(row, false);
    }
}

async function handleSave(row, id) {
    const cells = row.cells;
    const checkbox = row.querySelector(".js-married");
    const saveBtn = row.querySelector(".js-save");

    const data = {
        Id: id,
        Name: cells[0].innerText.trim(),
        DateOfBirth: cells[1].innerText.trim(),
        Married: checkbox.checked,
        Phone: cells[3].innerText.trim(),
        Salary: cells[4].innerText.replace(',', '.')
    };

    saveBtn.disabled = true;
    const success = await handleRequest("/Home/Update", {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    });

    if (success) {
        showToast("Save successfully", "success");
    } else {
        saveBtn.disabled = false;
    }
}

async function handleRequest(url, options) {

    try {

        const response = await fetch(url, options);

        const contentType = response.headers.get("content-type");

        if (!contentType || !contentType.includes("application/json")) {

            if (response.ok) return true;

            throw new Error(`Server status: ${response.status}`);

        }

        const result = await response.json();

        if (response.ok) return true;



        let msg = result.message || "Error occured";

        if (Array.isArray(result.errors)) msg += ": " + result.errors.join(", ");

        throw new Error(msg);

    } catch (error) {

        console.error("API Error:", error);

        showToast(error.message, "error");

        return false;

    }

}

function showToast(message, type = 'error') {
    const container = document.getElementById('toast-container');
    if (!container) return;

    const toast = document.createElement('div');
    toast.className = `toast-msg ${type}`;
    toast.innerText = message;

    container.appendChild(toast);

    requestAnimationFrame(() => {
        toast.classList.add("show");
    });

    setTimeout(() => {
        toast.classList.remove("show");
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}