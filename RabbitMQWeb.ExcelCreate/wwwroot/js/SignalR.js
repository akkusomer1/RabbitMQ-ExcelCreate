
$(document).ready(function () {
    var connection = new signalR.HubConnectionBuilder().withUrl("/MyHub").build();

    start();
    function start() {
        connection.start().then().catch((err) => {
            console.log(err)
            setTimeout(() => start(), 2000)
        });
    }

    connection.on("CompletedFile", (data) => {
        console.log("Excel oluşturma  işlemi bitti.")

        userFile = jQuery.parseJSON(data)
        console.log(userFile)

        Swal.fire({
            position: 'top-end',
            icon: 'success',
            title: 'Excel dosyanız hazır...',
            showConfirmButton: true,
            confirmButtonText: "Dosyalarım",

        }).then((result) => {
            if (result.isConfirmed) {
                window.location.href = "/product/files";
            }

        })

        const row = $("#fileTable").find(`[name="${userFile.Id}"]`)
        if (row != null) {

            row.fadeOut(1500);

            const newRow = `
            <tr name="${userFile.Id}">
                <td>${userFile.FileName}</td>
                <td>${userFile.GetCreatedDate}</td>
                <td>Oluşturuldu</td>
                <td>
                    <a href="~/files/${userFile.FilePath}" download="" class="btn btn-primary">Download</a>
                </td>
            </tr>         
            `
         
            const objectRow = $(newRow);

            objectRow.hide();
            row.replaceWith(objectRow);

            objectRow.fadeIn(1500);
       
        }
    });
})

       