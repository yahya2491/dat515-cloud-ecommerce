"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState, useContext } from "react";
import { UserContext } from "../../context/UserContext";
import {
  getUserCart,
  updateCartItemQuantity,
  removeItemFromCart,
} from "../../utils/requests";
import styles from "../page.module.css";

export default function CartPage() {
  const router = useRouter();
  const { user, initialized } = useContext(UserContext);
  const [cart, setCart] = useState(null);
  const [loading, setLoading] = useState(true);
  const [updating, setUpdating] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!initialized || !user?.id) return;

    async function fetchCart() {
      try {
        const data = await getUserCart(user.id);
        setCart(data);
      } catch (err) {
        console.error(err);
        setError("Failed to load cart");
      } finally {
        setLoading(false);
      }
    }

    fetchCart();
  }, [initialized, user]);

  async function handleQuantityChange(productId, newQuantity) {
  if (newQuantity < 1) return;
  setUpdating(true);

  try {
    const item = cart.items.find((i) => i.productId === productId);
    if (!item) return;

    const diff = newQuantity - item.quantity; // calculate difference

    // if diff is 0, do nothing
    if (diff === 0) return;


    const updated = await updateCartItemQuantity(cart.id, productId, diff);
    setCart(updated);
  } catch (err) {
    console.error(err);
    alert("Failed to update quantity");
  } finally {
    setUpdating(false);
  }
}


  async function handleRemove(productId) {
    if (!confirm("Remove this item?")) return;
    try {
      const updated = await removeItemFromCart(cart.id, productId);
      setCart(updated);
    } catch (err) {
      console.error(err);
      alert("Failed to remove item");
    }
  }

  if (!initialized || loading)
    return <p style={{ textAlign: "center", marginTop: "2rem" }}>Loading...</p>;
  if (error)
    return <p style={{ textAlign: "center", color: "red", marginTop: "2rem" }}>{error}</p>;

  const totalAmount = cart?.items?.reduce(
    (sum, item) => sum + (item.unitPrice || 0) * (item.quantity || 0),
    0
  );

  return (
    <main className={styles.cartPage}>
      <h1>Your Cart</h1>

      {cart && cart.items?.length > 0 ? (
        <>
          <ul className={styles.cartList}>
            {cart.items.map((item) => (
              <li key={item.productId} className={styles.cartItem}>
                <div className={styles.itemInfo}>
                  <strong>{item.productName}</strong>
                  <p>${(item.unitPrice || 0).toFixed(2)} each</p>
                </div>
                <div className={styles.itemActions}>
                  Qty:{" "}
                  <input
                    type="number"
                    value={item.quantity}
                    min="1"
                    onChange={(e) =>
                      handleQuantityChange(item.productId, parseInt(e.target.value))
                    }
                    disabled={updating}
                  />
                  <button onClick={() => handleRemove(item.productId)}>Remove</button>
                </div>
              </li>
            ))}
          </ul>

          <h3 className={styles.cartTotal}>Total: ${totalAmount.toFixed(2)}</h3>

          <button
            onClick={() => router.push("/checkout")}
            className={styles.checkoutBtn}
            disabled={!cart || !cart.items?.length}
          >
            Proceed to Checkout
          </button>
        </>
      ) : (
        <p>Your cart is empty.</p>
      )}
    </main>
  );
}
