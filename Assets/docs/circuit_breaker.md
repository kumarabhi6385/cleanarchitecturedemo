# Circuit Breaker Pattern

## **What is the Circuit Breaker Pattern?**

![Alt text](../images/circuit_breaker_state.png)

The Circuit Breaker Pattern is a design pattern used in software development, especially in distributed systems and microservices, to improve system resilience and fault tolerance. It prevents cascading failures by monitoring service health and dynamically detecting failures. When a service or dependency becomes unresponsive or fails repeatedly, the circuit breaker "trips" to stop further requests, allowing the system to recover gracefully and avoid resource exhaustion[1][2][5][7][8].

---

## **States of a Circuit Breaker**

A circuit breaker typically operates in three main states[1][3][5][6][7]:

| State     | Description                                                                                     |
| --------- | ----------------------------------------------------------------------------------------------- |
| Closed    | Normal operation. Requests flow freely. If failures exceed a threshold, transitions to Open.    |
| Open      | Requests are immediately failed or fallback is triggered. After a timeout, moves to Half-Open.  |
| Half-Open | Allows a limited number of test requests. If successful, returns to Closed; else, back to Open. |

### **State Transitions**

- **Closed → Open:** Triggered when the number of failures within a time window exceeds the configured threshold.
- **Open → Half-Open:** After a specified timeout, the breaker allows limited traffic to test if the service has recovered.
- **Half-Open → Closed:** If test requests succeed, normal operation resumes.
- **Half-Open → Open:** If test requests fail, the breaker returns to the Open state.

---

## **Key Parameters**

- **Failure Threshold:** Number or percentage of failures before opening the circuit.
- **Timeout (Open State):** How long to wait before transitioning from Open to Half-Open.
- **Test Request Limit (Half-Open):** Number of requests allowed in Half-Open state to test recovery.
- **Rolling Window:** Time window over which failures are counted.
- **Fallback Mechanism:** Optional logic to execute when the circuit is open.
- **Monitoring/Metrics:** For observing circuit breaker behavior and service health.

---

## **Flow of the Circuit Breaker Pattern**

1. **Normal Operation (Closed):** All requests pass through. Failures are monitored.
2. **Failure Detected:** If failures exceed the threshold, the circuit breaker opens.
3. **Open State:** Requests fail fast or fallback is used. No calls to the failing service.
4. **Timeout Elapses:** Circuit breaker transitions to Half-Open.
5. **Half-Open State:** Limited test requests are sent.
   - If successful, circuit closes (normal operation resumes).
   - If failures persist, circuit reopens.
6. **Monitoring:** Throughout, failures and successes are tracked for decision-making.

---

## **Circuit Breaker in Polly (.NET)**

[Polly](https://github.com/App-vNext/Polly) is a popular .NET resilience and transient-fault-handling library that provides a robust implementation of the Circuit Breaker pattern.

**How Polly Implements Circuit Breaker:**

- **Configuration:** Developers specify the number of exceptions allowed before breaking, the duration of the break, and the number of test calls in the Half-Open state.
- **States:** Polly manages Closed, Open, and Half-Open states automatically.
- **Usage Example:**
  ```
  var circuitBreakerPolicy = Policy
      .Handle<Exception>()
      .CircuitBreaker(
          exceptionsAllowedBeforeBreaking: 3,
          durationOfBreak: TimeSpan.FromSeconds(30)
      );
  ```
- **Fallback:** Polly allows integration with fallback, retry, and timeout policies.
- **Metrics:** Exposes events for monitoring state transitions and failures.

---

## **When to Use the Circuit Breaker Pattern**

- When interacting with **remote services** or dependencies that may become slow or unavailable.
- In **distributed systems** to prevent cascading failures and resource exhaustion.
- Where **high resilience and fault tolerance** are required.
- When you need to **fail fast** and provide fallback responses to maintain user experience.

---

## **When Not to Use the Circuit Breaker Pattern**

- For **local, in-memory operations** with negligible failure risk.
- In **simple applications** where failure impact is minimal and complexity is not justified.
- When the cost of occasional failures is acceptable and does not threaten system stability.

---

## **Benefits**

- **Prevents cascading failures** and system overload.
- **Improves resilience** and fault tolerance in distributed systems.
- **Faster failure response:** Users get immediate feedback instead of waiting for timeouts.
- **Supports fallback strategies** for degraded but graceful service.
- **Enables system recovery** by periodically testing the health of dependencies.

---

## **Drawbacks**

- **Added complexity** in system design and configuration.
- **Risk of false positives:** Circuit may open due to transient or partial failures, affecting healthy services.
- **Requires careful tuning** of

**References:**  
https://martinfowler.com/bliki/CircuitBreaker.html
