$(document).ready(function () {
    // Guest Search functionality for adding multiple guests
    $('#searchGuestAddModal').on('input', function () {
        var searchTerm = $(this).val();
        if (searchTerm.length > 2) {
            $.get('/api/GuestApi/SearchGuests', { searchTerm: searchTerm }, function (data) {
                $('#addGuestSearchResults').empty();
                $.each(data, function (index, guest) {
                    var row = `<tr>
                                    <td>${guest.guestId}</td>
                                    <td>${guest.firstName}</td>
                                    <td>${guest.lastName}</td>
                                    <td>${guest.company}</td>
                                    <td><button type="button" class="btn btn-primary select-guest-btn" data-guest-id="${guest.guestId}">Select</button></td>
                               </tr>`;
                    $('#addGuestSearchResults').append(row);
                });

                // Attach click event to select guest button
                $('.select-guest-btn').on('click', function () {
                    $('#comResGuestId').val($(this).data('guest-id'));
                });
            });
        }
    });

    // Add selected guest to the list
    $('#addGuestToReservation').on('click', function () {
        var guestId = $('#comResGuestId').val();
        var preference = $('#preference').val();

        if (guestId) {
            var listItem = `<li class="list-group-item">${guestId} - ${preference} <button type="button" class="btn btn-danger btn-sm float-end remove-guest-btn">Remove</button></li>`;
            $('#addedGuestsList').append(listItem);

            // Attach remove functionality
            $('.remove-guest-btn').last().on('click', function () {
                $(this).closest('li').remove();
            });

            // Clear the selected guest ID
            $('#comResGuestId').val('');
            $('#addGuestsModal').modal('hide');
        }
    });

    // Save added guests to hidden inputs for form submission
    function saveAddedGuests() {
        var guestInputs = '';
        $('#addedGuestsList li').each(function () {
            var guestId = $(this).text().split(' - ')[0];
            var preference = $(this).text().split(' - ')[1].trim();
            guestInputs += `<input type="hidden" name="GuestIds" value="${guestId}" />`;
            guestInputs += `<input type="hidden" name="Preferences" value="${preference}" />`;
        });
        $('#addedGuestsHiddenInputs').html(guestInputs);
    }

    // Call saveAddedGuests before submitting the form
    $('form').on('submit', function () {
        saveAddedGuests();
    });
});
