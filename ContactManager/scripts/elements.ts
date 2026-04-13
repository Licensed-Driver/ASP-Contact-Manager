class UpdateContactModal extends HTMLElement {
    constructor() {
        super();
    }

    connectedCallback() {

        this.style.display = 'none';

        this.innerHTML =`
            <div class="position-fixed top-0 start-0 w-100 h-100 bg-dark bg-opacity-50 z-3 d-flex justify-content-center align-items-center">
                <div class="card shadow-sm w-100" style="max-width: 500px;">
                    <div class="card-header bg-primary text-white">
                        <h5 class="mb-0">Update Contact</h5>
                    </div>
                    <div class="card-body">
                        <form id="updateContactForm">
                            <div class="mb-3">
                                <label class="form-label">First Name *</label>
                                <input type="text" class="form-control" id="update-firstName" required />
                                <div class="text-danger small mt-1" id="err-Update-FirstName"></div>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">Last Name</label>
                                <input type="text" class="form-control" id="update-lastName" />
                            </div>
                            <div class="mb-3">
                                <label class="form-label">Email</label>
                                <input type="email" class="form-control" id="update-email" />
                                <div class="text-danger small mt-1" id="err-Update-Email"></div>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">Phone</label>
                                <input type="text" class="form-control" id="update-phone" />
                                <div class="text-danger small mt-1" id="err-Update-Phone"></div>
                            </div>
                            <div class="text-danger small mb-3" id="err-Update-General"></div>
                            <div class="d-flex gap-3 mb-3">
                                <button type="button" class="btn btn-danger w-100" id="cancelUpdateButton">Cancel</button>
                                <button type="submit" class="btn btn-primary w-100">Save Contact</button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>`
    }
}

class Documentation extends HTMLElement {
    constructor() {
        super();
    }

    connectedCallback() {
        const getContent = async () => {
            const response = await fetch("/Documentation", { method: 'GET' });
            if (response.ok) {
                const html = await response.text();
                console.log(html);
                this.innerHTML = html;
            } else {
                this.innerHTML = "<p class='text-danger'>Failed to load docs.</p>";
            }
        }

        getContent();
    }
}

customElements.define('update-contact-modal', UpdateContactModal);
customElements.define('documentation-container', Documentation);