var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function  () {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url":"/Admin/User/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "company.name", "width": "15%" },
            { "data": "role", "width": "15%" },
            {
                "data": {
                    id: "id", lockoutEnd:"lockoutEnd"
                    },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        //user locked
                        return `
                         <div class ="text-center">
                         <a class="btn btn-danger" onclick=Lockunlock('${data.id}')>
                        unLock
                        </a>
                        </div>
                        `;
                    }
                        else {
                        //user unlock
                        return `
                        <div class="text-center">
                        <a class="btn btn-success" onclick=Lockunlock('${data.id}')>
                         Lock
                        </a>
                        </div>
                        `;
                    }
                }
            }

        ]
    })
}
function Lockunlock(id) {
    alert(id);
}