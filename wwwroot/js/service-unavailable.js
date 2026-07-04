(function retry() {
  fetch(location.href)
    .then((response) => {
      if (response.ok) {
        location.reload();
      }
    })
    .catch(() => {});

  setTimeout(retry, 3000);
})();
