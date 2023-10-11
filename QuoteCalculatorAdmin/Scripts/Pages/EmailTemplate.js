var DataTable = $('#tblemailtemplateList').DataTable();

$(document).ready(function () {
    $("#menu_EmailTemplate").addClass("active");
    BindTemplateList();
});
function BindTemplateList() {

    try {
        if (DataTable != 'undefined') {
            DataTable.destroy();
        }
        DataTable = $('#tblemailtemplateList').DataTable({
            colReorder: {
                enable: false
            },
            serverSide: true,
            //processing: true,
            language: {
                'processing': '<div class="loader"></div>'
            },
            ajax: {
                url: '/EmailTemplate/GetAllTemplatesList',
                type: 'POST',
                //data: function (d) {
                //    d.ImportQuoteId = 10;
                //}
            },
            "lengthMenu": [10, 20,50, 100, 200, 500],
            "pageLength": 10,
            "order": [],
            //dom: 'Blfrtip',
            "bFilter": true,
            //"aaData": data.Data,
            "aoColumns": [

                {
                    "mData": "ServiceName",
                    "className": "text-left",
                },
                {
                    "mData": "Subject",
                    "className": "text-left"
                },
                {
                    "mData": "HtmlContent",
                    "className": "text-left",
                    "render": function (data, type, row, meta) {
                        var result = '<div class="table-overflow">' + row.HtmlContent + '</div>';
                        return result;
                    }
                    
                },
                {
                    "mData": "id",
                    "bSortable": false,
                    "className": "text-center",
                    "render": function (data, type, row, meta) {
                        var ActionButtonEdit = '<a href="/EmailTemplate/ManageTemplates?templateId=' + row.id +'" title="Edit"  class="btn pr-2 edit-icon" ><i class="fa-regular fa-pen-to-square"></i></a>';
                        var ActionButtonDelete = '<a href="javascript:void(0)" onclick="DeleteTemplate(' + row.id + ')" title="Delete"class="btn delete-icon"><i class="fa-regular fa-trash-can"></i></a>';
                        var result = '';
                        result = result + ActionButtonEdit;
                        result = result + '&nbsp;&nbsp;&nbsp;' + ActionButtonDelete;
                        return result;
                    }
                }

            ],
        });
        $(".dataTables_length").css('clear', 'none');
        $(".dataTables_length").css('margin-right', '20px');
        $(".dataTables_info").css('clear', 'none');
        $(".dataTables_info").css('padding', '0');
    } catch (e) {
        alert(e.message);
    }
}
function DeleteTemplate(EmailTemplateId) {
    bootbox.confirm({
        message: "Are you sure you want to delete template?",
        buttons: {
        cancel: {
            label: "No",
            className: "btn-cancel min-auto cancel"
        },
        confirm: {
            label: "Yes",
            className: "btn-warning min-auto  confirm"
        }
        }, callback: function (result) {
            if (result) {
                $.ajax({
                    url: '/EmailTemplate/Destroy',
                    async: false,
                    type: 'GET',
                    data: { EmailTemplateId: Number(EmailTemplateId) },
                    success: function (data) {
                        if (data && data.IsSuccess) {
                            toastr.success(data.Message);
                            BindTemplateList();
                        }
                        else {
                            toastr.error(data.Message);
                        }
                    }
                });
            }
        }
    });
}