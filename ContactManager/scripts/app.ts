// Sync frontend and backend types
interface Contact {
    id?: string;
    firstName: string;
    lastName?: string;
    email?: string;
    phone?: string;
}
// Keep a stateful copy of contacts
let currentContacts: Array<Contact> = [];

function renderTable(contacts: Array<Contact>) {
    const tableBody = document.getElementById('contactTableBody');
    if (tableBody == null) return;
    tableBody.innerHTML = "";

    if (contacts.length == 0) {
        tableBody.innerHTML = "<tr><td colspan='4' class='text-center text-muted'> No Contacts Found.</td></tr>"
        return;
    }

    contacts.forEach(c => {
        // Create each element and set its content to avoid XSS
        const row = document.createElement('tr');
        const name = document.createElement('td');
        const first = document.createElement('strong');
        first.textContent = c.firstName;
        name.appendChild(first);
        if (c.lastName) name.appendChild(document.createTextNode(" " + c.lastName));
        row.appendChild(name);
        const email = document.createElement('td');
        if (c.email) email.textContent = c.email;
        else {
            const noField = document.createElement('span');
            noField.appendChild(document.createTextNode("--"));
            noField.classList.add('text-muted');
            email.appendChild(noField);
        }
        row.appendChild(email);
        const phone = document.createElement('td');
        if (c.phone) phone.textContent = c.phone;
        else {
            const noField = document.createElement('span');
            noField.appendChild(document.createTextNode("--"));
            noField.classList.add('text-muted');
            phone.appendChild(noField);
        }
        row.appendChild(phone);
        // Safe to inject here since id is standardized and server-side
        const buttons = document.createElement('td');
        buttons.innerHTML = `
    <button onclick="showUpdateModal(true, '${c.id}')" class="btn btn-sm btn-primary">Update</button>
    <button onclick="deleteContact('${c.id}')" class="btn btn-sm btn-danger">Delete</button>
`;
        row.appendChild(buttons);

        tableBody.appendChild(row);
    })
}

async function loadContacts() {
    const response = await fetch('/Contact/GetAll');
    if (response.ok) {
        currentContacts = await response.json();
        renderTable(currentContacts);
    }
    else {
        console.error(response);
    }
}

async function searchContacts(query) {
    if (!query) return loadContacts();  // Gives us an option to load all

    const response = await fetch(`/Contact/Search?query=${encodeURIComponent(query)}&page=0&size=20`);
    if (response.ok) {
        currentContacts = await response.json();
        renderTable(currentContacts);
    }
    else {
        console.error(response);
    }
}

async function tryAddNewContact(event: Event) {
    event.preventDefault();
    clearErrors();

    // Grab the info
    const newContact: Contact = {
        firstName: (<HTMLInputElement>document.getElementById("firstName")).value,
        lastName: (<HTMLInputElement>document.getElementById("lastName")).value,
        email: (<HTMLInputElement>document.getElementById("email")).value,
        phone: (<HTMLInputElement>document.getElementById("phone")).value
    }

    // POST the request
    const response = await fetch("/Contact/Add",
        {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newContact)
        });

    if (response.ok) {
        // Response good to we add it
        (<HTMLFormElement>document.getElementById("addContactForm")).reset();
        loadContacts();
    } else if (response.status === 400) {
        // Since we used ModelState errors and named our form fields the same, we can just pass it right back
        const errorData = await response.json();
        displayErrors(errorData.errors);
    }
}

async function deleteContact(id) {
    if (!confirm("Are you sure you want to delete this contact?")) return;

    const response = await fetch(`/Contact/Delete/${id}`, { method: 'DELETE' });
    if (response.ok) {
        loadContacts(); // Refresh the table without reloading page
    }
    else {
        console.error(response);
    }
}

function showUpdateModal(show: boolean, id?:string) {
    const modal = document.getElementById("updateContactModal");
    if (!modal) return;

    // Set the values to the current values of provided contact
    if (id) {
        const contact = currentContacts.find(c => c.id == id);  // Grab it from our list

        if (!contact) return;

        (<HTMLFormElement>document.getElementById("updateContactForm")).dataset.id = contact.id; // Inject the Id for grabbing info in method below
        (<HTMLInputElement>document.getElementById("update-firstName")).value = contact.firstName;
        (<HTMLInputElement>document.getElementById("update-lastName")).value = contact.lastName ? contact.lastName : "";
        (<HTMLInputElement>document.getElementById("update-email")).value = <string>contact.email;
        (<HTMLInputElement>document.getElementById("update-phone")).value = <string>contact.phone;
    }

    modal.style.display = show ? 'block' : 'none';
}

async function tryUpdateContact(event: Event) {
    event.preventDefault();
    clearErrors();

    // Grab the info
    const newContact: Contact = {
        id: (<HTMLFormElement>document.getElementById("updateContactForm")).dataset.id,
        firstName: (<HTMLInputElement>document.getElementById("update-firstName")).value,
        lastName: (<HTMLInputElement>document.getElementById("update-lastName")).value,
        email: (<HTMLInputElement>document.getElementById("update-email")).value,
        phone: (<HTMLInputElement>document.getElementById("update-phone")).value
    }

    // POST the request
    const response = await fetch("/Contact/Update",
        {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newContact)
        });

    if (response.ok) {
        // Response good so we reload the list
        showUpdateModal(false);
        (<HTMLFormElement>document.getElementById("updateContactForm")).reset();
        loadContacts();
    } else if (response.status === 400) {
        // Since we used ModelState errors and named our form fields the same, we can just pass it right back
        const errorData = await response.json();
        displayErrors(errorData.errors, true);
    }
}

function displayErrors(errorData, inUpdate?:boolean) {
    for (const key in errorData) {
        if (key === "") {
            // Show all errors if there's no specific targer
            const errorElement = document.getElementById(`err${inUpdate ? "-Update" : ""}-General`);
            if (errorElement) errorElement.innerText = errorData[key].join(", ");
        } else {
            // Otherwise show it on the target itself
            const errorElement = document.getElementById(`err${inUpdate ? "-Update" : ""}-${key}`);
            if (errorElement) errorElement.innerText = errorData[key].join(", ");
        }
    }
}

function clearErrors() {
    document.querySelectorAll(".text-danger").forEach(element => (<HTMLDivElement>element).innerText = "");
}

document.addEventListener("DOMContentLoaded", () => {
    loadContacts();

    (<HTMLFormElement>document.getElementById("addContactForm")).addEventListener("submit", tryAddNewContact);
    (<HTMLFormElement>document.getElementById("updateContactForm")).addEventListener("submit", tryUpdateContact);
    (<HTMLButtonElement>document.getElementById("cancelUpdateButton")).addEventListener("click", () => { showUpdateModal(false) });

    // Debounce the search obviously
    let searchDebounceTime;
    (<HTMLInputElement>document.getElementById("searchInput")).addEventListener("input", (e:Event) => {
        clearTimeout(searchDebounceTime);
        searchDebounceTime = setTimeout(() => {
            searchContacts((<HTMLInputElement>e.target).value);
        }, 300)
    })
})