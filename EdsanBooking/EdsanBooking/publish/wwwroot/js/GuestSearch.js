$(document).ready(function () {
    // Guest Search functionality
    $('#guestSearchInput').on('input', function () {
        var searchTerm = $(this).val();
        if (searchTerm.length > 2) {
            $.get('/api/GuestApi/SearchGuests', { searchTerm: searchTerm }, function (data) {
                $('#guestSearchResults').empty();
                $.each(data, function (index, guest) {
                    var row = `<tr>
                                    <td>${guest.guestId}</td>
                                    <td>${guest.firstName}</td>
                                    <td>${guest.lastName}</td>
                                    <td>${guest.company}</td>
                                    <td><button type="button" class="btn btn-primary select-guest-btn" data-guest-id="${guest.guestId}">Select</button></td>
                               </tr>`;
                    $('#guestSearchResults').append(row);
                });
            });
        }
    });

    // Attach select guest event
    $(document).on('click', '.select-guest-btn', function () {
        $('#GuestId').val($(this).data('guest-id'));
        $('#guestSearchModal').modal('hide');

        // Remove backdrop and reset body classes after modal is closed
        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open');
    });
});
