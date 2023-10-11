var DataTable = $('#tblMastercontentList').DataTable();

$(document).ready(function () {
    $("#menu_ContentMaster").addClass("active");
    $("#menu_Content").children().removeClass("collapsed");
    $("#menu_Content").children().addClass("show");
    $("#menu_Content").addClass("active");
    BindMasterContentList();
});
//function HeadingContentEdit(e) {
//    if (e != 0) {
//        e.preventDefault();
//        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
//        var HeadingContentId = dataItem.HeadingContentId;
//    }
//    HeadingContentPopUp(HeadingContentId);
//}

function BindMasterContentList() {
    
    try {
        if (DataTable != 'undefined') {
            DataTable.destroy();
        }
        DataTable = $('#tblMastercontentList').DataTable({
            colReorder: {
                enable: false
            },
            serverSide: true,
            //processing: true,
            language: {
                'processing': '<div class="loader"></div>'
            },
            ajax: {
                url: "/ContentMaster/GetContentList",
                type: 'POST',

            },
            "lengthMenu": [10,20, 50, 100, 200, 500],
            "pageLength": 10,
            "order": [],
            //dom: 'Blfrtip',
            "bFilter": true,

            //"aaData": data.Data,
            "aoColumns": [

                {
                    "mData": "Heading",
                    "className": "text-left",
                    "render": function (data, type, row, meta) {
                        var result = '<div class="text-overflow ">' + row.Heading + '</div>';

                        return result;
                    }
                },
                {
                    "mData": "HeadingContent",
                    "className": "text-left ",
                    "render": function (data, type, row, meta) {
                        var result = '<div class="table-overflow ">' + row.HeadingContent + '</div>';

                        return result;
                    }
                },

                {
                    "mData": "CountryCode",
                    "className": "text-left",
                    "render": function (data, type, row, meta) {
                        var result = '<div class="text-overflow ">' + row.CountryCode + '</div>';
                        return result;
                    }
                },
                {
                    "mData": "QuoteType",
                    "className": "text-left",
                    "render": function (data, type, row, meta) {
                        var result = '<div class="text-overflow ">' + row.QuoteType + '</div>';
                        return result;
                    }
                },
                {
                    "mData": "Company",
                    "className": "text-left"
                },
                {
                    "mData": "DisplayOrder",
                    "className": "text-left"

                },

                {
                    "mData": "HeadingContentId",
                    "bSortable": false,
                    "className": "text-center",
                    "render": function (data, type, row, meta) {
                        var ActionButtonEdit = '<a href="javascript:void(0)" title="Edit" onclick="openheadingContentModal(' + row.HeadingContentId + ')"  data-toggle="modal" data-target="#usermodalPopup" class="btn pr-2 edit-icon"><i class="fa-regular fa-pen-to-square"></i></a>';
                        var ActionButtonDelete = '<a href="javascript:void(0)" onclick="DeleteHeadingContent(' + row.HeadingContentId + ')" title="Delete" class="btn delete-icon"><i class="fa-regular fa-trash-can"></i></a>';
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

function openheadingContentModal(id) {
    $.ajax({
        url: '/ContentMaster/ManageContent',
        async: false,
        type: 'GET',
        data: { contentId: id },
        success: function (data) {
            $('#contentmodalPopup').modal('show');
            $('#divContentmodalPopup').html(data);
        }
    });
}
function DeleteHeadingContent(Id) {
    bootbox.confirm({
        message: "Are You Sure You Want To Delete Quote?",
        buttons: {
            cancel: {
                label: "No",
                className: "btn-cancel min-auto cancel"
            },
            confirm: {
                label: "Yes, Delete ",
                className: "btn-warning min-auto confirm"
            }
        },
        callback: function (result) {
            if (result) {
                $.ajax({
                    type: 'POST',
                    url: '/ContentMaster/Destroy',
                    dataType: 'json',
                    data: { Id: Id },
                    cache: false,
                    async: false,
                    success: function (data) {
                        if (data) {
                            toastr.success(data.Message);
                            //$('#BaggageQuoteGrid').data('kendoGrid').dataSource.read();
                            BindMasterContentList();
                        }
                    },
                    error: function () {
                        alert("We are facing some problem please try again later");
                        //$('#BaggageQuoteGrid').data('kendoGrid').dataSource.read();
                    }
                });
            }

        }
    });
}