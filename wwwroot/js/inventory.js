let currentFilter = "all";

function animateRows() {
  document.querySelectorAll("#inv-body .admin-table tbody tr").forEach((row, index) => {
    row.style.animationDelay = `${index * 24}ms`;
    row.classList.add("fade-in");
  });
}

async function loadInventory(filter, page) {
  const body = document.getElementById("inv-body");
  if (!body) return;

  const params = new URLSearchParams({ filter, page: page ?? 1 });

  body.style.opacity = "0.4";

  try {
    const res = await fetch(`/AdminInventory/InventoryPartial?${params}`);
    const html = await res.text();
    body.innerHTML = html;
    body.style.opacity = "1";
    animateRows();
    bindTableEvents();
  } catch {
    body.style.opacity = "1";
  }
}

function bindFilterButtons() {
  document.querySelectorAll("#inv-filters .seg-btn").forEach(btn => {
    btn.addEventListener("click", () => {
      document.querySelectorAll("#inv-filters .seg-btn").forEach(b => b.classList.remove("active"));
      btn.classList.add("active");
      currentFilter = btn.dataset.filter;
      loadInventory(currentFilter, 1);
    });
  });
}

function bindPagination() {
  document.querySelectorAll("#inv-body .pagination a").forEach(a => {
    a.addEventListener("click", e => {
      e.preventDefault();
      const url = new URL(a.href);
      const page = url.searchParams.get("page") || 1;
      loadInventory(currentFilter, page);
    });
  });
}

function bindTableEvents() {
  bindPagination();

  document.querySelectorAll(".btn-quick").forEach(btn => {
    btn.addEventListener("click", async () => {
      const productId = btn.dataset.productId;
      const operation = btn.dataset.operation;
      const row = btn.closest("tr");
      const stockCell = row?.querySelector("td:nth-child(3)");
      const statusCell = row?.querySelector("td:nth-child(4)");

      btn.disabled = true;

      try {
        const form = new URLSearchParams();
        form.set("productId", productId);
        form.set("operation", operation);

        const res = await fetch("/AdminInventory/QuickAdjust", {
          method: "POST",
          headers: { "Content-Type": "application/x-www-form-urlencoded" },
          body: form
        });

        const data = await res.json();

        if (data.success && stockCell) {
          stockCell.textContent = data.newStock;
          if (statusCell && currentFilter !== "discontinued") {
            statusCell.textContent = data.newStock <= 0 ? "Out" : "Active";
          }
        }
      } finally {
        btn.disabled = false;
      }
    });
  });

  document.querySelectorAll(".toggle-disc").forEach(cb => {
    cb.addEventListener("change", async () => {
      const productId = cb.dataset.productId;
      const row = cb.closest("tr");
      const statusCell = row?.querySelector("td:nth-child(4)");

      cb.disabled = true;

      try {
        const form = new URLSearchParams();
        form.set("productId", productId);

        const res = await fetch("/AdminInventory/ToggleDiscontinued", {
          method: "POST",
          headers: { "Content-Type": "application/x-www-form-urlencoded" },
          body: form
        });

        const data = await res.json();

        if (data.success && statusCell) {
          statusCell.textContent = data.newValue ? "Discontinued" : "Active";
          cb.checked = data.newValue;
        } else {
          cb.checked = !cb.checked;
        }
      } finally {
        cb.disabled = false;
      }
    });
  });
}

// Init
animateRows();
bindFilterButtons();
bindTableEvents();
