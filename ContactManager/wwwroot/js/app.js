"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
// Keep a stateful copy of contacts
let currentContacts = [];
function renderTable(contacts) {
    const tableBody = document.getElementById('contactTableBody');
    if (tableBody == null)
        return;
    tableBody.innerHTML = "";
    if (contacts.length == 0) {
        tableBody.innerHTML = "<tr><td colspan='4' class='text-center text-muted'> No Contacts Found.</td></tr>";
        return;
    }
    contacts.forEach(c => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td><strong>${c.firstName}</strong> ${c.lastName || ""}</td>
            <td>${c.email || "<span class='text-muted'>--</span>"}</td>
            <td>${c.phone || "<span class='text-muted'>--</span>"}</td>
            <td>
                <button onclick="showUpdateModal(true, '${c.id}')" class="btn btn-sm btn-primary">Update</button>
                <button onclick="deleteContact('${c.id}')" class="btn btn-sm btn-danger">Delete</button>
            </td>
        `;
        tableBody.appendChild(row);
    });
}
function loadContacts() {
    return __awaiter(this, void 0, void 0, function* () {
        const response = yield fetch('/Contact/GetAll');
        if (response.ok) {
            currentContacts = yield response.json();
            renderTable(currentContacts);
        }
        else {
            console.error(response);
        }
    });
}
function searchContacts(query) {
    return __awaiter(this, void 0, void 0, function* () {
        if (!query)
            return loadContacts(); // Gives us an option to load all
        const response = yield fetch(`/Contact/Search?query=${encodeURIComponent(query)}&page=0&size=20`);
        if (response.ok) {
            currentContacts = yield response.json();
            renderTable(currentContacts);
        }
        else {
            console.error(response);
        }
    });
}
function tryAddNewContact(event) {
    return __awaiter(this, void 0, void 0, function* () {
        event.preventDefault();
        clearErrors();
        // Grab the info
        const newContact = {
            firstName: document.getElementById("firstName").value, // @ts-ignore
            lastName: document.getElementById("lastName").value, // @ts-ignore
            email: document.getElementById("email").value, // @ts-ignore
            phone: document.getElementById("phone").value
        };
        // POST the request
        const response = yield fetch("/Contact/Add", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newContact)
        });
        if (response.ok) {
            // Response good to we add it
            // @ts-ignore
            document.getElementById("addContactForm").reset();
            loadContacts();
        }
        else if (response.status === 400) {
            // Since we used ModelState errors and named our form fields the same, we can just pass it right back
            const errorData = yield response.json();
            displayErrors(errorData.errors);
        }
    });
}
function deleteContact(id) {
    return __awaiter(this, void 0, void 0, function* () {
        if (!confirm("Are you sure you want to delete this contact?"))
            return;
        const response = yield fetch(`/Contact/Delete/${id}`, { method: 'DELETE' });
        if (response.ok) {
            loadContacts(); // Refresh the table without reloading page
        }
        else {
            console.error(response);
        }
    });
}
function showUpdateModal(show, id) {
    const modal = document.getElementById("updateContactModal");
    if (!modal)
        return;
    // Set the values to the current values of provided contact
    if (id) {
        const contact = currentContacts.find(c => c.id == id); // Grab it from our list
        // @ts-ignore
        document.getElementById("updateContactForm").dataset.id = contact.id; // @ts-ignore Inject the Id for grabbing info in method below
        document.getElementById("update-firstName").value = contact.firstName; // @ts-ignore
        document.getElementById("update-lastName").value = contact.lastName; // @ts-ignore
        document.getElementById("update-email").value = contact.email; // @ts-ignore
        document.getElementById("update-phone").value = contact.phone;
    }
    modal.style.display = show ? 'block' : 'none';
}
function tryUpdateContact(event) {
    return __awaiter(this, void 0, void 0, function* () {
        event.preventDefault();
        clearErrors();
        // Grab the info
        const newContact = {
            id: document.getElementById("updateContactForm").dataset.id, // @ts-ignore
            firstName: document.getElementById("update-firstName").value, // @ts-ignore
            lastName: document.getElementById("update-lastName").value, // @ts-ignore
            email: document.getElementById("update-email").value, // @ts-ignore
            phone: document.getElementById("update-phone").value
        };
        // POST the request
        const response = yield fetch("/Contact/Update", {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newContact)
        });
        if (response.ok) {
            // Response good so we reload the list
            showUpdateModal(false);
            // @ts-ignore
            document.getElementById("updateContactForm").reset();
            loadContacts();
        }
        else if (response.status === 400) {
            // Since we used ModelState errors and named our form fields the same, we can just pass it right back
            const errorData = yield response.json();
            displayErrors(errorData.errors, true);
        }
    });
}
function displayErrors(errorData, inUpdate) {
    for (const key in errorData) {
        if (key === "") {
            // Show all errors if there's no specific targer
            const errorElement = document.getElementById(`err${inUpdate ? "-Update" : ""}-General`);
            if (errorElement)
                errorElement.innerText = errorData[key].join(", ");
        }
        else {
            // Otherwise show it on the target itself
            const errorElement = document.getElementById(`err${inUpdate ? "-Update" : ""}-${key}`);
            if (errorElement)
                errorElement.innerText = errorData[key].join(", ");
        }
    }
}
function clearErrors() {
    // @ts-ignore
    document.querySelectorAll(".text-danger").forEach(element => element.innerText = "");
}
document.addEventListener("DOMContentLoaded", () => {
    loadContacts();
    // @ts-ignore
    document.getElementById("addContactForm").addEventListener("submit", tryAddNewContact);
    // @ts-ignore
    document.getElementById("updateContactForm").addEventListener("submit", tryUpdateContact);
    // @ts-ignore
    document.getElementById("cancelUpdateButton").addEventListener("click", () => { showUpdateModal(false); });
    // Debounce the search obviously
    let searchDebounceTime;
    // @ts-ignore
    document.getElementById("searchInput").addEventListener("input", (e) => {
        clearTimeout(searchDebounceTime);
        searchDebounceTime = setTimeout(() => {
            // @ts-ignore
            searchContacts(e.target.value);
        }, 300);
    });
});
//# sourceMappingURL=app.js.map